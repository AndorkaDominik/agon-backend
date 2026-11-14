using AutoMapper;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;
using Transfer.Dtos;

namespace BLL.Services;

public class DiscussionService(
    ApplicationDbContext context, 
    IMapper mapper,
    UserService userService
    )
{
    private readonly ApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly UserService _userService = userService;

    private async Task EnsureUserHasAccessAsync(int discussionId, string userId)
    {
        var hasAccess = await _context.Discussions.AnyAsync(d =>
            d.Id == discussionId &&
            (d.Athlete.UserId == userId || d.Trainer.UserId == userId));

        if (!hasAccess)
            throw new UnauthorizedAccessException("Nincs jogosultságod ehhez a beszélgetéshez.");
    }

    public async Task EnsureUserCanAccessDiscussionAsync(int discussionId, ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        await EnsureUserHasAccessAsync(discussionId, user!.Id);
    }

    public async Task<DiscussionDto> EnsureDiscussionAsync(int athleteId, int trainerId)
    {
        var existingDiscussion = await _context.Discussions
            .Include(d => d.Athlete).ThenInclude(a => a.User)
            .Include(d => d.Trainer).ThenInclude(t => t.User)
            .FirstOrDefaultAsync(d =>
                d.AthleteId == athleteId &&
                d.TrainerId == trainerId);

        if (existingDiscussion != null)
        {
            return _mapper.Map<DiscussionDto>(existingDiscussion);
        }

        var discussion = new Discussion
        {
            AthleteId = athleteId,
            TrainerId = trainerId,
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = null
        };

        _context.Discussions.Add(discussion);
        await _context.SaveChangesAsync();

        var savedDiscussion = await _context.Discussions
            .Include(d => d.Athlete).ThenInclude(a => a.User)
            .Include(d => d.Trainer).ThenInclude(t => t.User)
            .FirstAsync(d => d.Id == discussion.Id);

        return _mapper.Map<DiscussionDto>(savedDiscussion);
    }

    public async Task<List<DiscussionDto>> GetMyDiscussionsAsync(ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var discussions = await _context.Discussions
            .Include(d => d.Athlete).ThenInclude(a => a.User)
            .Include(d => d.Trainer).ThenInclude(t => t.User)
            .Include(d => d.Messages)
            .Where(d => d.Athlete.UserId == user.Id || d.Trainer.UserId == user.Id)
            .OrderByDescending(d => d.LastMessageAt ?? d.CreatedAt)
            .ToListAsync();

        var result = _mapper.Map<List<DiscussionDto>>(discussions);

        foreach (var dto in result)
        {
            var discussion = discussions.First(d => d.Id == dto.Id);

            dto.UnreadCount = discussion.Messages
                .Count(m => m.SenderId != user.Id && !m.IsRead);
        }

        return result;
    }

    public async Task<List<DiscussionMessageDto>> GetMessagesAsync(int discussionId,
        ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        await EnsureUserHasAccessAsync(discussionId, user.Id);

        var messages = await _context.DiscussionMessages
            .Include(m => m.Sender)
            .Where(m => m.DiscussionId == discussionId)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        return _mapper.Map<List<DiscussionMessageDto>>(messages);
    }

    public async Task<SendDiscussionMessageResultDto> SendMessageAsync(
        int discussionId,
        string content,
        ClaimsPrincipal principal
    )
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("The message cannot be empty.");

        var discussion = await _context.Discussions
            .Include(d => d.Athlete).ThenInclude(a => a.User)
            .Include(d => d.Trainer).ThenInclude(t => t.User)
            .FirstOrDefaultAsync(d => d.Id == discussionId);

        if (discussion == null)
            throw new KeyNotFoundException("Discussion not found.");

        var isAthlete = discussion.Athlete.UserId == user.Id;
        var isTrainer = discussion.Trainer.UserId == user.Id;

        if (!isAthlete && !isTrainer)
            throw new UnauthorizedAccessException("You do not have permission to have this conversation.");

        var receiverUserId = isAthlete
            ? discussion.Trainer.UserId
            : discussion.Athlete.UserId;

        var message = new DiscussionMessage
        {
            DiscussionId = discussionId,
            SenderId = user.Id,
            Content = content.Trim(),
            SentAt = DateTime.UtcNow,
            IsRead = false,
        };

        discussion.LastMessageAt = message.SentAt;

        _context.DiscussionMessages.Add(message);
        await _context.SaveChangesAsync();

        await _context.Entry(message).Reference(m => m.Sender).LoadAsync();

        return new SendDiscussionMessageResultDto
        {
            Message = _mapper.Map<DiscussionMessageDto>(message)
        };
    }

    public async Task MarkAsReadAsync(int discussionId, ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        await EnsureUserHasAccessAsync(discussionId, user.Id);

        var unreadMessages = await _context.DiscussionMessages
            .Where(m => m.DiscussionId == discussionId && m.SenderId != user.Id && !m.IsRead)
            .ToListAsync();

        foreach (var message in unreadMessages)
            message.IsRead = true;

        await _context.SaveChangesAsync();
    }
}
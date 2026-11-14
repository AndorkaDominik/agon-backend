using AutoMapper;
using BLL.Exceptions;
using BLL.Validators;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Transfer.Dtos;

namespace BLL.Services;

public class AthleteService(ApplicationDbContext context,
    UserService userService,
    IMapper mapper
    )
{
    private readonly ApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly UserService _userService = userService;

    public async Task<UpdateThemeColorDto> UpdateThemeColorAsync(UpdateThemeColorDto request, ClaimsPrincipal principal)
    {
        var validator = new UpdateThemeColorDtoValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var athlete = await _context.Athletes.FirstOrDefaultAsync(a => a.UserId == user!.Id);
        if (athlete == null)
        {
            throw new AthleteNotFoundException();
        }

        athlete.ThemeColor = request.ThemeColor;
        await _context.SaveChangesAsync();
        return new UpdateThemeColorDto { ThemeColor = athlete.ThemeColor };
    }

    public async Task<CoachingRequestDto> RequestCoachingAsync(CoachingRequestDto requestDto, ClaimsPrincipal principal)
    {
        var validator = new CoachingRequestValidator();
        var validationResult = validator.Validate(requestDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var athlete = await _context.Athletes.FirstOrDefaultAsync(a => a.UserId == user.Id);

        if (athlete == null)
        {
            throw new AthleteNotFoundException();
        }

        var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Id == requestDto.TrainerId);
        if (trainer == null)
        {
            throw new TrainerNotFoundException();
        }
        
        var coachingRequest = _mapper.Map<CoachingRequest>(requestDto);
        coachingRequest.AthleteId = athlete.Id;
        coachingRequest.CreatedAt = DateTime.UtcNow;
        _context.CoachingRequests.Add(coachingRequest);
        await _context.SaveChangesAsync();

        var savedRequest = await _context.CoachingRequests
            .Include(cr => cr.Athlete)
                .ThenInclude(a => a.User)
            .Include(cr => cr.Trainer)
                .ThenInclude(t => t.User)
            .FirstOrDefaultAsync(cr => cr.Id == coachingRequest.Id);

        if (savedRequest == null)
        {
            throw new Exception("Saved coaching request could not be loaded.");
        }

        return _mapper.Map<CoachingRequestDto>(savedRequest);
    }

    public async Task<AthleteDto> AssignTrainerToAthleteAsync(int athleteId, int trainerId)
    {
        var athlete = await _context.Athletes.FirstOrDefaultAsync(a => a.Id == athleteId);
        if (athlete == null) throw new AthleteNotFoundException();

        athlete.TrainerId = trainerId;
        await _context.SaveChangesAsync();

        var updated = await _context.Athletes
            .Include(a => a.User)
            .Include(a => a.AthleteProgress)
            .Include(a => a.Trainer).ThenInclude(t => t.User)
            .FirstOrDefaultAsync(a => a.Id == athleteId);

        return _mapper.Map<AthleteDto>(updated);
    }

    public async Task<AthleteDto?> UnassignTrainerFromAthleteAsync(int athleteId)
    {
        var athlete = await _context.Athletes.FirstOrDefaultAsync(a => a.Id == athleteId);
        if (athlete == null) throw new AthleteNotFoundException();

        athlete.TrainerId = null;
        await _context.SaveChangesAsync();

        var updated = await _context.Athletes
            .Include(a => a.User)
            .Include(a => a.AthleteProgress)
            .Include(a => a.Trainer).ThenInclude(t => t.User)
            .FirstOrDefaultAsync(a => a.Id == athleteId);

        return _mapper.Map<AthleteDto>(updated);
    }

    public async Task<List<WorkoutPlanDto>> GetAssignedWorkoutPlansAsync(ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var athlete = await _context.Athletes
            .FirstOrDefaultAsync(a => a.UserId == user.Id);

        if (athlete == null) throw new AthleteNotFoundException();

        var workoutPlans = await _context.WorkoutPlans
            .Include(p => p.Exercises)
            .Include(p => p.SharedWithAthletes)
            .Where(p => p.SharedWithAthletes.Any(ap => ap.AthleteId == athlete.Id))
            .ToListAsync();

        return _mapper.Map<List<WorkoutPlanDto>>(workoutPlans);
    }

    public async Task<AthleteDto> UnassignTrainerAsync(ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var athlete = await _context.Athletes.FirstOrDefaultAsync(a => a.UserId == user.Id);
        if (athlete == null) throw new AthleteNotFoundException();

        if (athlete.TrainerId == null)
        {
            var current = await _context.Athletes
                .Include(a => a.User)
                .Include(a => a.AthleteProgress)
                .Include(a => a.Trainer).ThenInclude(t => t.User)
                .FirstOrDefaultAsync(a => a.Id == athlete.Id);

            return _mapper.Map<AthleteDto>(current!);
        }
        var updated = await UnassignTrainerFromAthleteAsync(athlete.Id);
        if (updated == null || updated.TrainerId != null)
        {
            throw new Exception("Failed to disconnect athlete.");
        }
        return updated;
    }
}

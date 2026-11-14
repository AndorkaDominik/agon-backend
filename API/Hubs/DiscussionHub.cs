using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BLL.Services;

namespace API.Hubs;

[Authorize]
public class DiscussionHub(DiscussionService discussionService) : Hub
{
    public async Task JoinDiscussion(int discussionId)
    {
        if (Context.User is null)
            throw new HubException("You must be signed in to join a discussion.");

        try
        {
            await discussionService.EnsureUserCanAccessDiscussionAsync(discussionId, Context.User);
        }
        catch (UnauthorizedAccessException)
        {
            throw new HubException("You do not have permission to join this discussion.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"discussion-{discussionId}");
    }

    public Task LeaveDiscussion(int discussionId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"discussion-{discussionId}");
}

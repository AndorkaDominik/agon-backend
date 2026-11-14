using API.Hubs;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Transfer.Dtos;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DiscussionController(
    DiscussionService discussionService,
    IHubContext<DiscussionHub> hubContext
) : ControllerBase
{
    private readonly DiscussionService _discussionService = discussionService;
    private readonly IHubContext<DiscussionHub> _hubContext = hubContext;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<DiscussionDto>>> GetMyDiscussions()
    {
        var discussions = await _discussionService.GetMyDiscussionsAsync(User);
        return Ok(discussions);
    }

    [HttpGet("{discussionId}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<DiscussionMessageDto>>> GetMessages(int discussionId)
    {
        var messages = await _discussionService.GetMessagesAsync(discussionId, User);
        return Ok(messages);
    }

    [HttpPost("{discussionId}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DiscussionMessageDto>> SendMessage(
        int discussionId,
        [FromBody] SendDiscussionMessageRequestDto dto)
    {
        var result = await _discussionService.SendMessageAsync(
            discussionId,
            dto.Content,
            User
        );

        await _hubContext.Clients.Group($"discussion-{discussionId}")
            .SendAsync("DiscussionMessageReceived", result.Message);

        return Ok(result.Message);
    }

    [HttpPost("{discussionId}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(int discussionId)
    {
        await _discussionService.MarkAsReadAsync(discussionId, User);

        await _hubContext.Clients.Group($"discussion-{discussionId}")
            .SendAsync("DiscussionMarkedAsRead", discussionId);

        return NoContent();
    }
}

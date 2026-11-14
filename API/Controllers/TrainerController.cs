using API.Hubs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Transfer.Dtos;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainerController(
    TrainerService trainerService,
    IHubContext<DiscussionHub> hubContext
) : ControllerBase
{
    private readonly TrainerService _trainerService = trainerService;
    private readonly IHubContext<DiscussionHub> _hubContext = hubContext;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<TrainerDto>>> GetTrainers([FromQuery] string? search = null)
    {
        var trainers = await _trainerService.GetTrainersAsync(search);
        return Ok(trainers);
    }

    [HttpGet("paginated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TrainerPaginationResponseDto>> GetTrainersWithPagination(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null)
    {
        var response = await _trainerService.GetTrainersWithPaginationAsync(limit, offset, search);
        return Ok(response);
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TrainerDashboardDto>> GetTrainerDashboard()
    {
        var dashboardData = await _trainerService.GetTrainerDashboardAsync(User);
        return Ok(dashboardData);
    }


    [HttpGet("manage/{athleteId}/")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ManageAthleteDataResponseDto>> GetManageAthleteData(int athleteId)
    {
        var response = await _trainerService.GetManageAthleteDataAsync(athleteId, User);
        return Ok(response);
    }

    [HttpPost("requests/{requestId}/respond")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CoachingRequestResponseResultDto>> RespondToCoachingRequest(int requestId, [FromQuery] bool accept)
    {
        var result = await _trainerService.RespondToCoachingRequestAsync(requestId, accept, User);

        if (accept && result.Discussion != null && result.AthleteUserId != null)
        {
            await _hubContext.Clients.User(result.AthleteUserId)
                .SendAsync("DiscussionCreated", result.Discussion);
        }

        return Ok(result);
    }

    [HttpPost("disconnect/{athleteId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DisconnectAthlete(int athleteId)
    {
        await _trainerService.DisconnectAthleteAsync(athleteId, User);
        return Ok(new { message = "Athlete disconnected." });
    }
}

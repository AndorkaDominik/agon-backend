using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transfer.Dtos;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(policy: "Athlete")]
public class AthleteController(AthleteService athleteService) : ControllerBase
{
    private readonly AthleteService _athleteService = athleteService;

    [HttpPost("request-coaching")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CoachingRequestDto>> RequestCoaching([FromBody] CoachingRequestDto requestDto)
    {
        var response = await _athleteService.RequestCoachingAsync(requestDto, User);
        return Created(nameof(response), response);
    }

    [HttpPost("disconnect-trainer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AthleteDto>> DisconnectTrainer()
    {
        var updatedAthlete = await _athleteService.UnassignTrainerAsync(User);
        return Ok(updatedAthlete);
    }

    [HttpGet("error")]
    public Task GetError()
    {
        throw new ArgumentNullException(nameof(GetError));
    }

    [HttpPatch("theme")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateThemeColorDto>> UpdateThemeColor([FromBody] UpdateThemeColorDto request)
    {
        var response = await _athleteService.UpdateThemeColorAsync(request, User);
        return Ok(response);
    }

}
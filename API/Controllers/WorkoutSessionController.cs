using BLL.Services;
using BLL.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transfer.Dtos;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Athlete")]
public class WorkoutSessionController(WorkoutSessionService workoutService) : ControllerBase
{
    private readonly WorkoutSessionService _workoutSessionService = workoutService;

    [HttpPost("new-workout")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreateWorkoutResponseDto>> CreateWorkout([FromBody] WorkoutSessionDto workoutSessionDto)
    {
        try
        {
            var workout = await _workoutSessionService.CreateWorkoutAsync(workoutSessionDto, User);
            if (workout == null)
            {
                return BadRequest("Failed to create workout");
            }

            return Created(nameof(workout), workout);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                message = "Validation failed.",
                errors = ex.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage })
            });
        }
    }

    [HttpPost("xp-reward")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutXpRewardDto>> CalculateXpReward([FromBody] WorkoutSessionDto workoutSessionDto)
    {
        if (workoutSessionDto?.Exercises == null)
        {
            return BadRequest("Workout data is required.");
        }

        var xpReward = await _workoutSessionService.CalculateWorkoutXpPreviewAsync(workoutSessionDto, User);
        return Ok(xpReward);
    }

    [HttpGet("last-workout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutSessionDto>> GetLastWorkout()
    {
        var workout = await _workoutSessionService.GetLastWorkoutAsync(User);
        return Ok(workout);
    }

    [HttpGet("activity-week")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ActivityWeekResponseDto>> GetActivityWeek()
    {
        var activityWeekDto = await _workoutSessionService.GetActivityWeekAsync(User);
        return Ok(activityWeekDto);
    }

    [HttpGet("dashboard-progress")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AthleteDashboardProgressDto>> GetDashboardProgress()
    {
        var dashboardProgress = await _workoutSessionService.GetDashboardProgressAsync(User);
        return Ok(dashboardProgress);
    }
}

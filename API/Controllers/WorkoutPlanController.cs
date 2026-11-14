using BLL.Exceptions;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transfer.Dtos;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutPlanController(
    WorkoutPlanService workoutPlanService,
    AthleteService athleteService
    ) : ControllerBase
{
    public readonly WorkoutPlanService _workoutPlanService = workoutPlanService;
    public readonly AthleteService _athleteService = athleteService;

    [HttpGet("plans")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<WorkoutPlanDto>>> GetAthleteWorkoutPlans()
    {
        var plans = await _workoutPlanService.GetWorkoutPlansAsync(User);
        return Ok(plans);
    }

    [HttpGet("templates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<WorkoutPlanDto>>> GetTemplateWorkoutPlans()
    {
        var plans = await _workoutPlanService.GetTemplatePlansAsync(User);
        return Ok(plans);
    }

    [HttpGet("{planId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutPlanDto>> GetPlanById(int planId)
    {
        var plan = await _workoutPlanService.GetPlanByIdAsync(planId, User);
        return Ok(plan);
    }

    [HttpGet("assigned-plans")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<WorkoutPlanDto>>> GetAssignedrWorkoutPlans()
    {
        var plans = await _athleteService.GetAssignedWorkoutPlansAsync(User);
        return Ok(plans);
    }

    [HttpPost("new-plan")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutPlanDto>> CreateNewPlan([FromBody] WorkoutPlanDto workoutPlanDto)
    {
        try
        {
            var plan = await _workoutPlanService.CreateNewPlanAsync(workoutPlanDto, User);
            return Created(nameof(plan), plan);
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

    [HttpPost("assign-plan")]
    [Authorize(Roles = "Trainer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutPlanDto>> AssignPlan([FromBody] AssignPlanRequestDto assignPlanDto)
    {
        var plan = await _workoutPlanService.AssignPlanAsync(assignPlanDto, User);
        return Ok(plan);
    }

    [HttpPut("{planId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutPlanDto>> UpdatePlan(int planId, [FromBody] WorkoutPlanDto workoutPlanDto)
    {
        var plan = await _workoutPlanService.UpdatePlanAsync(planId, workoutPlanDto, User);
        return Ok(plan);
    }

    [HttpDelete("{planId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePlan(int planId)
    {
        await _workoutPlanService.DeletePlanAsync(planId, User);
        return NoContent();
    }

    [HttpDelete("assigned/{planId}")]
    [Authorize(Roles = "Trainer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAssignedPlan(int planId)
    {
        await _workoutPlanService.DeleteAssignedPlanAndWorkoutPlanAsync(planId, User);
        return NoContent();
    }
}

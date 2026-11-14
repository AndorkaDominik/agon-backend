using AutoMapper;
using BLL.Exceptions;
using BLL.Helpers;
using BLL.Validators;
using DAL;
using DAL.Entities;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Transfer.Dtos;

namespace BLL.Services;

public class WorkoutPlanService(
    ApplicationDbContext context,
    UserService userService,
    IMapper mapper)
{
    private readonly ApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly UserService _userService = userService;
    public async Task<List<WorkoutPlanDto>> GetWorkoutPlansAsync(ClaimsPrincipal principal)
    {
        // this wont give back assigned plans
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var workoutPlans = await _context.WorkoutPlans
            .Include(p => p.Exercises)
            .Where(plan => plan.CreatorId == user.Id)
            .ToListAsync();

        return _mapper.Map<List<WorkoutPlanDto>>(workoutPlans);
    }

    public async Task<List<WorkoutPlanDto>> GetTemplatePlansAsync(ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var templatePlans = await _context.WorkoutPlans
            .Include(p => p.Exercises)
            .Include(p => p.SharedWithAthletes)
            .Where(p => p.CreatorId == user.Id && p.IsTemplate == true)
            .ToListAsync();

        return _mapper.Map<List<WorkoutPlanDto>>(templatePlans);
    }

    public async Task<WorkoutPlanDto> GetPlanByIdAsync(int planId, ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);

        var workoutPlan = await _context.WorkoutPlans
            .Include(p => p.Exercises)
            .Include(p => p.SharedWithAthletes)
            .FirstOrDefaultAsync(p => p.Id == planId && p.CreatorId == user.Id);

        if (workoutPlan == null)
        {
            throw new KeyNotFoundException($"Workout plan with ID {planId} not found or you do not have permission to view it.");
        }

        return _mapper.Map<WorkoutPlanDto>(workoutPlan);
    }

    public async Task<WorkoutPlanDto> CreateNewPlanAsync(WorkoutPlanDto workoutPlanDto, ClaimsPrincipal principal)
    {
        var validator = new WorkoutPlanDtoValidator();
        ValidationResult validationResult = validator.Validate(workoutPlanDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var workoutPlan = _mapper.Map<WorkoutPlan>(workoutPlanDto);
        workoutPlan.CreatorId = user.Id;
        // Templatet csak a Trainer állíthat
        workoutPlan.IsTemplate = principal.IsInRole("Trainer");

        NormalizeExercisesHelper.NormalizePlanExercises(workoutPlan.Exercises);
        
        _context.WorkoutPlans.Add(workoutPlan);
        await _context.SaveChangesAsync();

        return _mapper.Map<WorkoutPlanDto>(workoutPlan);
    }

    public async Task<WorkoutPlanDto> AssignPlanAsync(AssignPlanRequestDto assignPlanDto, ClaimsPrincipal principal)
    {
        if (!principal.IsInRole("Trainer")) throw new UnauthorizedAccessException("Only trainers can assign workout plans.");

        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.Id);
        if (trainer == null) throw new TrainerNotFoundException();

        var sourcePlan = await _context.WorkoutPlans
            .Include(wp => wp.Exercises)
            .FirstOrDefaultAsync(p => p.Id == assignPlanDto.TemplateId && p.CreatorId == user.Id);

        if (sourcePlan == null) throw new KeyNotFoundException($"Workout plan template with ID {assignPlanDto.TemplateId} not found.");

        var athlete = await _context.Athletes.FirstOrDefaultAsync(a => a.Id == assignPlanDto.AthleteId);
        if (athlete == null) throw new AthleteNotFoundException();

        var assignedPlan = new WorkoutPlan
        {
            Name = sourcePlan.Name,
            FocusArea = sourcePlan.FocusArea,
            Description = sourcePlan.Description,
            CreatorId = user.Id,
            IsTemplate = false,
            Exercises = sourcePlan.Exercises
                .Select(e => new PlanExercise
                {
                    ExerciseName = e.ExerciseName,
                    ExerciseType = e.ExerciseType,
                    TargetWeight = e.TargetWeight,
                    TargetReps = e.TargetReps,
                    TargetDuration = e.TargetDuration,
                    TargetDistance = e.TargetDistance,
                    OrderIndex = e.OrderIndex
                })
                .ToList()
        };

        NormalizeExercisesHelper.NormalizePlanExercises(assignedPlan.Exercises);

        _context.WorkoutPlans.Add(assignedPlan);
        await _context.SaveChangesAsync();

        _context.AssignedPlans.Add(new AssignedPlan
        {
            AthleteId = assignPlanDto.AthleteId,
            WorkoutPlanId = assignedPlan.Id,
            AssignedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        return _mapper.Map<WorkoutPlanDto>(assignedPlan);
    }

    public async Task<WorkoutPlanDto> UpdatePlanAsync(int planId, WorkoutPlanDto workoutPlanDto, ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);

        var workoutPlan = await _context.WorkoutPlans
            .Include(p => p.Exercises)
            .FirstOrDefaultAsync(p => p.Id == planId && p.CreatorId == user.Id);

        if (workoutPlan == null)
        {
            throw new KeyNotFoundException($"Workout plan with ID {planId} not found or you do not have permission to edit it.");
        }

        workoutPlan.Name = workoutPlanDto.Name;
        workoutPlan.FocusArea = workoutPlanDto.FocusArea;
        workoutPlan.Description = workoutPlanDto.Description;
        workoutPlan.IsTemplate = workoutPlanDto.IsTemplate ?? workoutPlan.IsTemplate;

        _context.PlanExercises.RemoveRange(workoutPlan.Exercises);
        workoutPlan.Exercises = workoutPlanDto.Exercises.Select(e => new PlanExercise
        {
            ExerciseName = e.ExerciseName,
            ExerciseType = (DAL.Entities.ExerciseType)e.ExerciseType,
            TargetWeight = e.TargetWeight,
            TargetReps = e.TargetReps,
            TargetDuration = e.TargetDuration,
            TargetDistance = e.TargetDistance,
            OrderIndex = e.OrderIndex
        }).ToList();

        NormalizeExercisesHelper.NormalizePlanExercises(workoutPlan.Exercises);

        await _context.SaveChangesAsync();

        return _mapper.Map<WorkoutPlanDto>(workoutPlan);
    }

    public async Task<WorkoutPlanDto> DeletePlanAsync(int planId, ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var workoutPlan = await _context.WorkoutPlans.FirstOrDefaultAsync(p => p.Id == planId && p.CreatorId == user.Id);

        if (workoutPlan == null)
        {
            throw new KeyNotFoundException($"Workout plan with ID {planId} not found or you do not have permission to delete it.");
        }
        _context.WorkoutPlans.Remove(workoutPlan);
        await _context.SaveChangesAsync();

        return _mapper.Map<WorkoutPlanDto>(workoutPlan);
    }
    public async Task DeleteAssignedPlanAndWorkoutPlanAsync(int planId, ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);

        var workoutPlan = await _context.WorkoutPlans
            .Include(p => p.SharedWithAthletes)
            .FirstOrDefaultAsync(p => p.Id == planId && p.CreatorId == user.Id);

        if (workoutPlan == null) throw new KeyNotFoundException($"Assigned workout plan with ID {planId} not found or you do not have permission to delete it.");

        _context.AssignedPlans.RemoveRange(workoutPlan.SharedWithAthletes);
        _context.WorkoutPlans.Remove(workoutPlan);

        await _context.SaveChangesAsync();
    }
}
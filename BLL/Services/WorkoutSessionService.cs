using AutoMapper;
using BLL.Exceptions;
using BLL.Validators;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Transfer.Dtos;
using FluentValidation.Results;
using BLL.Helpers;

namespace BLL.Services;

public class WorkoutSessionService(
    ApplicationDbContext context,
    UserService userService,
    IMapper mapper,
    WorkoutXpCalculatorService workoutXpCalculatorService,
    AthleteProgressCalculatorService athleteProgressCalculatorService)
{
    private readonly ApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly UserService _userService = userService;
    private readonly WorkoutXpCalculatorService _workoutXpCalculatorService = workoutXpCalculatorService;
    private readonly AthleteProgressCalculatorService _athleteProgressCalculatorService = athleteProgressCalculatorService;

    public async Task<CreateWorkoutResponseDto> CreateWorkoutAsync(WorkoutSessionDto workoutSessionDto, ClaimsPrincipal principal)
    {
        var validator = new WorkoutSessionDtoValidator();
        ValidationResult validationResult = validator.Validate(workoutSessionDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var athlete = await _context.Athletes
            .Include(a => a.AthleteProgress)
            .FirstOrDefaultAsync(a => a.UserId == user.Id);

        if (athlete == null)
        {
            throw new AthleteNotFoundException();
        }

        var workoutSession = _mapper.Map<WorkoutSession>(workoutSessionDto);
        workoutSession.UserId = user.Id;
        workoutSession.AthleteId = athlete.Id;
        workoutSession.Date = DateTime.UtcNow;

        NormalizeExercisesHelper.NormalizeSessionExercises(workoutSession.Exercises);
        CalculateTotals(workoutSession);

        var workoutXp = _workoutXpCalculatorService.CalculateWorkoutXp(workoutSession);

        athlete.AthleteProgress.StrengthXp += workoutXp.StrengthXp;
        athlete.AthleteProgress.EnduranceXp += workoutXp.EnduranceXp;
        athlete.AthleteProgress.BodyControlXp += workoutXp.BodyControlXp;

        _context.WorkoutSessions.Add(workoutSession);
        await _context.SaveChangesAsync();

        return new CreateWorkoutResponseDto
        {
            Workout = _mapper.Map<WorkoutSessionDto>(workoutSession),
            XpGained = _mapper.Map<WorkoutXpRewardDto>(workoutXp)
        };
    }

    public async Task<WorkoutXpRewardDto> CalculateWorkoutXpPreviewAsync(WorkoutSessionDto workoutSessionDto, ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var athlete = await _context.Athletes
            .FirstOrDefaultAsync(a => a.UserId == user.Id);

        if (athlete == null)
        {
            throw new AthleteNotFoundException();
        }

        var workoutSession = _mapper.Map<WorkoutSession>(workoutSessionDto);
        workoutSession.Exercises ??= [];

        NormalizeExercisesHelper.NormalizeSessionExercises(workoutSession.Exercises);

        var xpResult = _workoutXpCalculatorService.CalculateWorkoutXp(workoutSession);

        return _mapper.Map<WorkoutXpRewardDto>(xpResult);
    }

    public async Task<WorkoutSessionDto> GetLastWorkoutAsync(ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var athlete = await _context.Athletes
            .FirstOrDefaultAsync(a => a.UserId == user.Id);

        if (athlete == null)
        {
            throw new AthleteNotFoundException();
        }

        var lastworkoutsession = await _context.WorkoutSessions
            .Include(s => s.Exercises)
            .OrderByDescending(s => s.Date)
            .FirstOrDefaultAsync(s => s.AthleteId == athlete.Id);

        if (lastworkoutsession == null)
        {
            throw new WorkoutSessionNotFoundException("No workout session found for the athlete.");
        }

        return _mapper.Map<WorkoutSessionDto>(lastworkoutsession);
    }

    public async Task<ActivityWeekResponseDto> GetActivityWeekAsync(ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var athlete = await _context.Athletes
            .FirstOrDefaultAsync(a => a.UserId == user.Id);
        if (athlete == null)
        {
            throw new AthleteNotFoundException();
        }
        var today = DateTime.UtcNow.Date;
        var fromDate = today.AddDays(-6);
        var toDate = today.AddDays(1);

        var workouts = await _context.WorkoutSessions
            .Include(ws => ws.Exercises)
            .Where(ws => ws.AthleteId == athlete.Id && ws.Date >= fromDate && ws.Date < toDate)
            .ToListAsync();

        var days = Enumerable.Range(0, 7)
            .Select(offset =>
            {
                var date = today.AddDays(-offset);
                var workout = workouts.FirstOrDefault(ws => ws.Date.Date == date);

                return workout != null ? _mapper.Map<ActivityDayDto>(workout)
                : new ActivityDayDto
                {
                    Date = date,
                    WorkoutName = null,
                    ExerciseCount = 0,
                    TotalReps = 0,
                    TotalWeight = 0.0,
                    TotalDistance = 0.0
                };
            })
            .ToList();

        return new ActivityWeekResponseDto { Days = days };
    }

    public async Task<AthleteDashboardProgressDto> GetDashboardProgressAsync(ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var athlete = await _context.Athletes
            .Include(a => a.AthleteProgress)
            .FirstOrDefaultAsync(a => a.UserId == user.Id);

        if (athlete == null)
        {
            throw new AthleteNotFoundException();
        }

        var athleteProgress = athlete.AthleteProgress;
        var calculatedProgress = _athleteProgressCalculatorService.GetAthleteProgress(
            athleteProgress.StrengthXp,
            athleteProgress.EnduranceXp,
            athleteProgress.BodyControlXp);

        return _mapper.Map<AthleteDashboardProgressDto>(calculatedProgress);
    }

    private static void CalculateTotals(WorkoutSession workoutSession)
    {
        workoutSession.TotalWeight = workoutSession.Exercises
            .Where(e => e.ExerciseType == DAL.Entities.ExerciseType.Strength)
            .Sum(e => (e.Weight ?? 0) * (e.Reps ?? 0));

        workoutSession.TotalDistance = workoutSession.Exercises
            .Where(e => e.ExerciseType == DAL.Entities.ExerciseType.Cardio)
            .Sum(e => e.Distance ?? 0);

        workoutSession.TotalReps = workoutSession.Exercises
            .Where(e => e.ExerciseType == DAL.Entities.ExerciseType.Bodyweight)
            .Sum(e => e.Reps ?? 0);
    }
}

using AutoMapper;
using BLL.Exceptions;
using DAL;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Transfer.Dtos;

namespace BLL.Services;

public class TrainerService(
    ApplicationDbContext context,
    UserService userService,
    AthleteService athleteService,
    IMapper mapper,
    DiscussionService discussionService
    )
{
    private readonly ApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly UserService _userService = userService;
    private readonly AthleteService _athleteService = athleteService;
    private readonly DiscussionService _discussionService = discussionService;

    public async Task<List<TrainerDto>> GetTrainersAsync(string? search)
    {
        var query = _context.Trainers.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t => t.Introduction.Contains(search) 
                                || t.Specialization.Contains(search) 
                                || t.User.UserName.Contains(search));
        }

        var trainers = await query.ToListAsync();
        return _mapper.Map<List<TrainerDto>>(trainers);
    }

    public async Task<TrainerPaginationResponseDto> GetTrainersWithPaginationAsync(int limit, int offset, string? search = null)
    {
        var query = _context.Trainers.Include(t => t.Athletes).Include(t => t.User).AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t => t.User.UserName.Contains(search) || t.Specialization.Contains(search));
        }

        var trainers = await query
            .OrderBy(t => t.Athletes.Count)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var trainerDtos = _mapper.Map<List<TrainerSearchDto>>(trainers);

        return new TrainerPaginationResponseDto
        {
            Trainers = trainerDtos
        };
    }

    public async Task<TrainerDashboardDto> GetTrainerDashboardAsync(ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.Id);
        if (trainer == null)
        {
            throw new TrainerNotFoundException();
        }

        var requests = await _context.CoachingRequests
            .Include(c => c.Athlete.User)
            .Include(c => c.Trainer.User)
            .Where(c => c.TrainerId == trainer.Id)
            .ToListAsync();

        var athletes = await _context.Athletes
            .Include(a => a.User)
            .Include(a => a.AthleteProgress)
            .Include(a => a.Trainer)
                .ThenInclude(t => t.User)
            .Include(a => a.WorkoutSessions)
                .ThenInclude(ws => ws.Exercises)
            .Include(a => a.WorkoutPlans)
                .ThenInclude(wp => wp.Exercises)
            .Include(a => a.AssignedPlans)
                .ThenInclude(ap => ap.WorkoutPlan)
                    .ThenInclude(wp => wp.Exercises)
            .Where(a => a.TrainerId == trainer.Id)
            .AsSplitQuery()
            .ToListAsync();

        var dashboardDto = new TrainerDashboardDto
        {
            Info = new TrainerDashboardInfoDto
            {
                AthletesCount = athletes.Count,
                PendingRequestsCount = requests.Count(r => r.Status == DAL.Entities.RequestStatus.Pending)
            },
            Requests = requests.Select(request => _mapper.Map<CoachingRequestDto>(request)).ToList(),
            MyAthletes = athletes.Select(athlete => _mapper.Map<AthleteDto>(athlete)).ToList()
        };

        return dashboardDto;
    }

    public async Task<ManageAthleteDataResponseDto> GetManageAthleteDataAsync(int athleteId, ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.Id);
        if (trainer == null) throw new TrainerNotFoundException();

        var athlete = await _context.Athletes
            .FirstOrDefaultAsync(a => a.Id == athleteId && a.TrainerId == trainer.Id);

        if (athlete == null)
        {
            throw new KeyNotFoundException("Athlete not found or not assigned to the authenticated trainer.");
        }

        // Assigned-plans
        var assignedPlans = await _context.AssignedPlans
            .Include(ap => ap.WorkoutPlan)
                .ThenInclude(wp => wp.Exercises)
            .Where(ap => ap.AthleteId == athleteId)
            .ToListAsync();

        var workoutPlans = assignedPlans
            .Select(ap => ap.WorkoutPlan)
            .GroupBy(wp => wp.Id)
            .Select(g => g.First())
            .ToList();

        var workoutPlanDtos = _mapper.Map<List<WorkoutPlanDto>>(workoutPlans);

        // Activity-week
        var today = DateTime.UtcNow.Date;
        var fromDate = today.AddDays(-6);
        var toDate = today.AddDays(1);

        var workouts = await _context.WorkoutSessions
            .Include(ws => ws.Exercises)
            .Where(ws => ws.AthleteId == athleteId && ws.Date >= fromDate && ws.Date < toDate)
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
                    TotalWeight = 0.0
                };
            })
            .ToList();

        var week = new ActivityWeekResponseDto { Days = days };

        return new ManageAthleteDataResponseDto
        {
            AssignedPlans = workoutPlanDtos,
            ActivityWeek = week
        };
    }

    public async Task<CoachingRequestResponseResultDto> RespondToCoachingRequestAsync(int requestId, bool accept, ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.Id);

        if (trainer == null) throw new TrainerNotFoundException();

        var request = await _context.CoachingRequests
            .Include(r => r.Athlete)
                .ThenInclude(a => a.User)
            .Include(r => r.Trainer)
                .ThenInclude(t => t.User)
            .FirstOrDefaultAsync(r => r.Id == requestId && r.TrainerId == trainer.Id);

        if (request == null) throw new KeyNotFoundException("Coaching request not found.");

        DiscussionDto? discussionDto = null;

        if (accept)
        {
            var updatedAthlete = await _athleteService.AssignTrainerToAthleteAsync(request.AthleteId, trainer.Id);
            if (updatedAthlete == null || updatedAthlete.TrainerId != trainer.Id)
            {
                throw new Exception("Failed to assign trainer to athlete.");
            }

            request.Status = DAL.Entities.RequestStatus.Accepted;

            discussionDto = await _discussionService.EnsureDiscussionAsync(
                request.AthleteId,
                trainer.Id
            );
        }
        else
        {
            request.Status = DAL.Entities.RequestStatus.Declined;
        }

        await _context.SaveChangesAsync();

        return new CoachingRequestResponseResultDto
        {
            Request = _mapper.Map<CoachingRequestDto>(request),
            Discussion = discussionDto,
            AthleteUserId = request.Athlete.UserId
        };
    }

    public async Task DisconnectAthleteAsync(int athleteId, ClaimsPrincipal principal)
    {
        var user = await _userService.GetAuthenticatedUserAsync(principal);
        var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.Id);

        if (trainer == null) throw new TrainerNotFoundException();

        var athlete = await _context.Athletes.FirstOrDefaultAsync(a => a.Id == athleteId && a.TrainerId == trainer.Id);
        if (athlete == null) throw new AthleteNotFoundException();

        var updated = await _athleteService.UnassignTrainerFromAthleteAsync(athleteId);
        if (updated == null || updated.TrainerId != null)
        {
            throw new Exception("Failed to disconnect athlete.");
        }
    }
}

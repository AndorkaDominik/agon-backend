using AutoMapper;
using BLL.Helpers;
using BLL.Models;
using BLL.Services;
using DAL.Entities;
using Transfer.Dtos;

namespace API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<User, AuthResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserType, opt => opt.Ignore())
            .ForMember(dest => dest.ThemeColor, opt => opt.Ignore());

        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        CreateMap<AthleteProgress, AthleteProgressDto>()
            .ConvertUsing((src, _, _) => MapAthleteProgress(src));

        CreateMap<SkillProgress, SkillProgressDto>()
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.XpToNextLevel, opt => opt.MapFrom(src => src.RequiredXp - src.CurrentXp));

        CreateMap<AthleteProgressResult, AthleteDashboardProgressDto>()
            .ForMember(dest => dest.OverallLevel, opt => opt.MapFrom(src => src.Level))
            .ForMember(dest => dest.Strength, opt => opt.MapFrom(src => src.Strength))
            .ForMember(dest => dest.Endurance, opt => opt.MapFrom(src => src.Endurance))
            .ForMember(dest => dest.BodyControl, opt => opt.MapFrom(src => src.BodyControl))
            .AfterMap((_, dest) =>
            {
                dest.Strength.Name = "Strength";
                dest.Endurance.Name = "Endurance";
                dest.BodyControl.Name = "BodyControl";
            });

        CreateMap<Athlete, AthleteDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => src.AthleteProgress))
            .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer != null && src.Trainer.User != null ? src.Trainer.User.UserName : string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Trainer, opt => opt.Ignore())
            .ForMember(dest => dest.AthleteProgress, opt => opt.Ignore());

        CreateMap<Athlete, ProfileDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => src.AthleteProgress))
            .ForMember(dest => dest.TrainerId, opt => opt.MapFrom(src => src.TrainerId))
            .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer != null && src.Trainer.User != null ? src.Trainer.User.UserName : null))
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Trainer != null ? src.Trainer.Specialization : null))
            .ForMember(dest => dest.Introduction, opt => opt.MapFrom(src => src.Trainer != null ? src.Trainer.Introduction : null))
            .ReverseMap()
            .ForMember(dest => dest.ThemeColor, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TrainerId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Trainer, opt => opt.Ignore())
            .ForMember(dest => dest.AthleteProgress, opt => opt.Ignore());

        CreateMap<Trainer, ProfileDto>()
            .ForMember(dest => dest.ThemeColor, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        CreateMap<WorkoutSession, WorkoutSessionDto>()
            .ForMember(d => d.Exercises, o => o.MapFrom(s => s.Exercises))
            .ReverseMap();

        CreateMap<XpResult, WorkoutXpRewardDto>();

        CreateMap<SessionExercise, SessionExerciseDto>()
            .ForMember(d => d.ExerciseType, o => o.MapFrom(s => (Transfer.Dtos.ExerciseType)s.ExerciseType))
            .ReverseMap()
            .ForMember(d => d.ExerciseType, o => o.MapFrom(s => (DAL.Entities.ExerciseType)s.ExerciseType))
            .AfterMap((src, dest) => NormalizeExercisesHelper.NormalizeSessionExercises([dest]));

        CreateMap<WorkoutSession, ActivityDayDto>()
            .ForMember(dest => dest.ExerciseCount, o => o.MapFrom(s => s.Exercises.Count))
            .ForMember(dest => dest.TotalWeight, o => o.MapFrom(s => s.Exercises.Where(e => e.ExerciseType == DAL.Entities.ExerciseType.Strength).Sum(e => (e.Weight ?? 0) * (e.Reps ?? 0))))
            .ForMember(dest => dest.TotalDistance, o => o.MapFrom(s => s.Exercises.Where(e => e.ExerciseType == DAL.Entities.ExerciseType.Cardio).Sum(e => e.Distance ?? 0)))
            .ForMember(dest => dest.TotalReps, o => o.MapFrom(s => s.Exercises.Where(e => e.ExerciseType == DAL.Entities.ExerciseType.Bodyweight).Sum(e => e.Reps ?? 0)));

        CreateMap<WorkoutPlan, WorkoutPlanDto>()
            .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.Exercises))
            .ForMember(dest => dest.SharedWithAthletes, opt => opt.MapFrom(src => src.SharedWithAthletes))
            .ReverseMap()
            .ForMember(dest => dest.Creator, opt => opt.Ignore());

        CreateMap<PlanExercise, PlanExerciseDto>()
            .ForMember(d => d.ExerciseType, o => o.MapFrom(s => (Transfer.Dtos.ExerciseType)s.ExerciseType))
            .ReverseMap()
            .ForMember(d => d.ExerciseType, o => o.MapFrom(s => (DAL.Entities.ExerciseType)s.ExerciseType))
            .AfterMap((src, dest) => NormalizeExercisesHelper.NormalizePlanExercises([dest]));

        CreateMap<AssignedPlan, AssignedPlanDto>().ReverseMap();

        CreateMap<CoachingRequest, CoachingRequestDto>()
            .ForMember(dest => dest.AthleteName, opt => opt.MapFrom(s => s.Athlete.User.UserName))
            .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(s => s.Trainer.User.UserName))
            .ForMember(dest => dest.TrainerId, opt => opt.MapFrom(src => src.TrainerId))
            .ForMember(dest => dest.AthleteId, opt => opt.MapFrom(src => src.AthleteId));

        CreateMap<CoachingRequestDto, CoachingRequest>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Athlete, opt => opt.Ignore())
            .ForMember(dest => dest.Trainer, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<Transfer.Dtos.RequestStatus, DAL.Entities.RequestStatus>()
            .ConvertUsing(src => (DAL.Entities.RequestStatus)src);
        CreateMap<DAL.Entities.RequestStatus, Transfer.Dtos.RequestStatus>()
            .ConvertUsing(src => (Transfer.Dtos.RequestStatus)src);

        CreateMap<Trainer, TrainerSearchDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.AthleteCount, opt => opt.MapFrom(src => src.Athletes.Count))
            .ReverseMap();

        CreateMap<Discussion, DiscussionDto>()
            .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src =>
                src.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => m.Content)
                    .FirstOrDefault()))
            .ForMember(dest => dest.UnreadCount, opt => opt.Ignore());

        CreateMap<DiscussionMessage, DiscussionMessageDto>()
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src =>
                src.Sender.UserName ?? "Unknown"));
    }

    private static AthleteProgressDto MapAthleteProgress(AthleteProgress? athleteProgress)
    {
        if (athleteProgress == null)
        {
            return new AthleteProgressDto();
        }

        var athleteProgressCalculatorService = new AthleteProgressCalculatorService();
        var calculatedProgress = athleteProgressCalculatorService.GetAthleteProgress(
            athleteProgress.StrengthXp,
            athleteProgress.EnduranceXp,
            athleteProgress.BodyControlXp);

        return new AthleteProgressDto
        {
            Id = athleteProgress.Id,
            Level = calculatedProgress.Level,
            StrengthXp = athleteProgress.StrengthXp,
            EnduranceXp = athleteProgress.EnduranceXp,
            BodyControlXp = athleteProgress.BodyControlXp
        };
    }
}

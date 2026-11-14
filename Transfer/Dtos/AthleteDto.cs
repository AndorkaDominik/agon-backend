namespace Transfer.Dtos;

public class AthleteDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public double? Height { get; set; }
    public double? Weight { get; set; }
    public DateTime? BirthDate { get; set; }
    public int? TrainerId { get; set; }
    public string TrainerName { get; set; } = string.Empty;
    public AthleteProgressDto Progress { get; set; } = new();
    public ICollection<WorkoutSessionDto> WorkoutSessions { get; set; } = new List<WorkoutSessionDto>();
    public ICollection<WorkoutPlanDto> WorkoutPlans { get; set; } = new List<WorkoutPlanDto>();
    public ICollection<AssignedPlanDto> AssignedPlans { get; set; } = new List<AssignedPlanDto>();
}
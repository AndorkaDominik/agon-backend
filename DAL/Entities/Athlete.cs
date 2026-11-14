namespace DAL.Entities;

public class Athlete
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public double? Height { get; set; }
    public double? Weight { get; set; }
    public DateTime? BirthDate { get; set; }
    public string ThemeColor { get; set; } = "grass";
    public int? TrainerId { get; set; }
    public Trainer? Trainer { get; set; }
    public int? AthleteProgressId { get; set; }
    public AthleteProgress AthleteProgress { get; set; } = new();
    public ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
    public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
    public ICollection<AssignedPlan> AssignedPlans { get; set; } = new List<AssignedPlan>();
}

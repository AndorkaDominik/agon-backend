namespace DAL.Entities;

public class AssignedPlan
{
    public int AthleteId { get; set; }
    public int WorkoutPlanId { get; set; }
    public Athlete Athlete { get; set; } = null!;
    public WorkoutPlan WorkoutPlan { get; set; } = null!;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
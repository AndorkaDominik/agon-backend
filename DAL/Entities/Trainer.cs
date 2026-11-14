namespace DAL.Entities;

public class Trainer
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public string Specialization { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;

    // Navigation Property
    public ICollection<Athlete> Athletes { get; set; } = new List<Athlete>();
    public ICollection<WorkoutPlan> CreatedPlans { get; set; } = new List<WorkoutPlan>();
}
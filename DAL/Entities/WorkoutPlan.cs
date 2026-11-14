namespace DAL.Entities;

public class WorkoutPlan
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string FocusArea { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatorId { get; set; } = string.Empty;
    public bool? IsTemplate { get; set; } = false;

    // Navigation Property
    public User Creator { get; set; } = null!;
    public ICollection<PlanExercise> Exercises { get; set; } = new List<PlanExercise>();
    public ICollection<AssignedPlan> SharedWithAthletes { get; set; } = new List<AssignedPlan>();
}
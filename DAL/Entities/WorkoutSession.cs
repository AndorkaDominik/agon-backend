namespace DAL.Entities;

public class WorkoutSession
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int AthleteId { get; set; }
    public string WorkoutName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Notes { get; set; }

    // strenth exercises total
    public double? TotalWeight { get; set; }

    // cardio exercises total
    public double? TotalDistance { get; set; }

    // bodyweight exercises total
    public double? TotalReps { get; set; }

    // Navigation Property
    public ICollection<SessionExercise> Exercises { get; set; } = new List<SessionExercise>();
    public Athlete? Athlete { get; set; }
}
namespace Transfer.Dtos;

public class WorkoutSessionDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string WorkoutName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public double? TotalWeight { get; set; }
    public double? TotalDistance { get; set; }
    public int? TotalReps { get; set; }
    public List<SessionExerciseDto> Exercises { get; set; } = new();
}
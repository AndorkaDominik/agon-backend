namespace Transfer.Dtos;

public class AssignedPlanDto
{
    public int AthleteId { get; set; }
    public int WorkoutPlanId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
namespace Transfer.Dtos;

public class CreateWorkoutResponseDto
{
    public WorkoutSessionDto Workout { get; set; } = new();
    public WorkoutXpRewardDto XpGained { get; set; } = new();
}
namespace Transfer.Dtos;

public class TrainerDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = null!;
    public string Specialization { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public ICollection<AthleteDto> Athletes { get; set; } = new List<AthleteDto>();
    public ICollection<WorkoutPlanDto> CreatedPlans { get; set; } = new List<WorkoutPlanDto>();
}
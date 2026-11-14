using DAL.Entities;

namespace Transfer.Dtos;

public class WorkoutPlanDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string FocusArea { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatorId { get; set; } = string.Empty;
    public bool? IsTemplate { get; set; } = false;
    public List<PlanExerciseDto> Exercises { get; set; } = new();
    public List<AssignedPlanDto> SharedWithAthletes { get; set; } = new();
}
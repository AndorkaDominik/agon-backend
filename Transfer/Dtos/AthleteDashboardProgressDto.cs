namespace Transfer.Dtos;

public class AthleteDashboardProgressDto
{
    public int OverallLevel { get; set; }
    public SkillProgressDto Strength { get; set; } = new();
    public SkillProgressDto Endurance { get; set; } = new();
    public SkillProgressDto BodyControl { get; set; } = new();
}

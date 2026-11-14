namespace Transfer.Dtos;

public class SkillProgressDto
{
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public int TotalXp { get; set; }
    public int CurrentXp { get; set; }
    public int RequiredXp { get; set; }
    public int XpToNextLevel { get; set; }
    public double ProgressPercentage { get; set; }
}

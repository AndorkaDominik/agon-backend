namespace BLL.Models;

public class SkillProgress
{
    public int Level { get; set; }
    public int TotalXp { get; set; }
    public int CurrentXp { get; set; }
    public int RequiredXp { get; set; }
    public double ProgressPercentage { get; set; }
}

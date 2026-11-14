namespace DAL.Entities;

public class AthleteProgress
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public Athlete Athlete { get; set; } = null!;
    public int StrengthXp { get; set; } = 0;
    public int EnduranceXp { get; set; } = 0;
    public int BodyControlXp { get; set; } = 0;
}

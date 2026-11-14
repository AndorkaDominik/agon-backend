namespace Transfer.Dtos;

public class AthleteProgressDto
{
    public int Id { get; set; }
    public int Level { get; set; } = 1;
    public int StrengthXp { get; set; }
    public int EnduranceXp { get; set; }
    public int BodyControlXp { get; set; }
}
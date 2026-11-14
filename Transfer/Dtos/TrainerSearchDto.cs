namespace Transfer.Dtos;

public class TrainerSearchDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = null!;
    public string Specialization { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public int AthleteCount { get; set; }
}

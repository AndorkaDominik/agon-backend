namespace Transfer.Dtos;

public class ProfileDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public double? Height { get; set; }
    public double? Weight { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ThemeColor { get; set; }
    public int? TrainerId { get; set; }
    public string? TrainerName { get; set; }
    public string? Specialization { get; set; }
    public string? Introduction { get; set; }
    public AthleteProgressDto Progress { get; set; } = new();
}

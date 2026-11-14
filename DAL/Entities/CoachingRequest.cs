namespace DAL.Entities;

public class CoachingRequest
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public int TrainerId { get; set; }
    public string Message { get; set; } = string.Empty;
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation Properties
    public Athlete Athlete { get; set; } = null!;
    public Trainer Trainer { get; set; } = null!;
}

public enum RequestStatus { Pending, Accepted, Declined }
namespace Transfer.Dtos;

public class CoachingRequestDto
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public string AthleteName { get; set; } = string.Empty;
    public int TrainerId { get; set; }
    public string TrainerName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum RequestStatus { Pending, Accepted, Declined }
namespace Transfer.Dtos;

public class CoachingRequestResponseResultDto
{
    public CoachingRequestDto Request { get; set; } = null!;
    public DiscussionDto? Discussion { get; set; }
    public string? AthleteUserId { get; set; }
}
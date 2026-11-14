namespace Transfer.Dtos;

public class DiscussionDto
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public int TrainerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; }
    
    // wont be mapped
    public string? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}
namespace Transfer.Dtos;

public class DiscussionMessageDto
{
    public int Id { get; set; }
    public int DiscussionId { get; set; }
    public string SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}
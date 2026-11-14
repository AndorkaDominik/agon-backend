namespace DAL.Entities;

public class DiscussionMessage
{
    public int Id { get; set; }
    public int DiscussionId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; }

    // Navigation Property
    public Discussion Discussion { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
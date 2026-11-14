namespace DAL.Entities;
public class Discussion
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public int TrainerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; }

    // Navigation Properties
    public Athlete Athlete { get; set; } = null!;
    public Trainer Trainer { get; set; } = null!;
    public ICollection<DiscussionMessage> Messages { get; set; } = new List<DiscussionMessage>();

}
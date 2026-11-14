using Microsoft.AspNetCore.Identity;

namespace DAL.Entities;

public class User : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Property
    public Athlete? Athlete { get; set; }
    public Trainer? Trainer { get; set; }
}
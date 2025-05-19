namespace CampusLove.Domain.Entities;

public class UserDislikes
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DislikedProfileId { get; set; }
    public DateTime DislikeDate { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Profile? DislikedProfile { get; set; }
}

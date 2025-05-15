namespace CampusLove.Domain.Entities;

public class UserLikes
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LikedProfileId { get; set; }
    public DateTime LikeDate { get; set; }
    public bool IsMatch { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Profile? LikedProfile { get; set; }
}

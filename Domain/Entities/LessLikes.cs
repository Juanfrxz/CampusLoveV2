namespace CampusLove.Domain.Entities
{
    public class LessLikes
    {
        public string Username { get; set; } = string.Empty;
        public int ProfileId { get; set; }
        public int TotalLikes { get; set; }
    }
} 
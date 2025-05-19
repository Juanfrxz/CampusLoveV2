namespace CampusLove.Domain.Entities
{
    public class MaxLikes
    {
        public string Username { get; set; } = string.Empty;
        public int ProfileId { get; set; }
        public int TotalLikes { get; set; }
    }
} 
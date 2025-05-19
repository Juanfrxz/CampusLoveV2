namespace CampusLove.Domain.Entities
{
    public class UserInterest
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int UsersCount { get; set; }
    }
} 
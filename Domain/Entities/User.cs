namespace CampusLove.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int ProfileId { get; set; }
        public DateTime Birthdate { get; set; }
        public int BonusLikes { get; set; }

        public List<Profile> Profile { get; set; } = new List<Profile>();
    }
}
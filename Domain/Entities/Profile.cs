namespace CampusLove.Domain.Entities
{
    public class Profile
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Identification { get; set; }
        public int GenderId { get; set; }
        public string? Slogan { get; set; }
        public int StatusId { get; set; }
        public DateTime createDate { get; set; }
        public int ProfessionId { get; set; }
        public int TotalLikes { get; set; }

        public string FullName => $"{Name} {LastName}";

        public User? User { get; set; }
        public List<InterestProfile> Details { get; set; } = new List<InterestProfile>();
    }
}
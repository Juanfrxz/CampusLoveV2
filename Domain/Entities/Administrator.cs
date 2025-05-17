namespace CampusLove.Domain.Entities
{
    public class Administrator
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Identification { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int ApplicationId { get; set; }
    }
} 
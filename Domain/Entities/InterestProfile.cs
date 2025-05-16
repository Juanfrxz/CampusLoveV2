namespace CampusLove.Domain.Entities
{
    public class InterestProfile
    {
        public int ProfileId { get; set; }
        public int InterestId { get; set; }

        public Interest? Interest { get; set; }
    }
} 
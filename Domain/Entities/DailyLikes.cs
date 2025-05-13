namespace CampusLove.Domain.Entities
{
    public class DailyLikes
    {
        public int Id { get; set; }
        public DateTime date { get; set; }
        public int ProfileId { get; set; }
        public int Number_Likes { get; set; }
        public bool status { get; set; }
    }
}
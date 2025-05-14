namespace CampusLove.Domain.Entities
{
    public class DailyLikes
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ProfileId { get; set; }
        public int Number_Likes { get; set; }
        public bool Status { get; set; }
    }
}
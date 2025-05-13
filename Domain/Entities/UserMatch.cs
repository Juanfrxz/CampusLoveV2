namespace CampusLove.Domain.Entities
{
    public class UserMatch 
    {
        public int Id { get; set; }
        public int User1_id { get; set; }
        public int User2_id { get; set; }
        public DateTime matchDate { get; set; }
    }
}

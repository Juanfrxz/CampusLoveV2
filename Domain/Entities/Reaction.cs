namespace CampusLove.Domain.Entities
{
    public enum Reaction_type
    {
        like,
        dislike
    }
    public class Reaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime reactionDate { get; set; }
        public int ProfileId { get; set; }
        public Reaction_type reaction_type { get; set; }
    }
}
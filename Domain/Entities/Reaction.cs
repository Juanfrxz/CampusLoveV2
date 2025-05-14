namespace CampusLove.Domain.Entities
{
    public enum ReactionType
    {
        like = 0,
        dislike = 1
    }
    public class Reaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime reactionDate { get; set; }
        public int ProfileId { get; set; }
        public ReactionType ReactionType { get; set; }
    }
}
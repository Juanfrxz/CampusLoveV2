using System;

namespace CampusLove.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
} 
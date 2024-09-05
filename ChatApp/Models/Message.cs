using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SenderID { get; set; }
        public DateTime Time { get; set; }
        public int ReplyID { get; set; }
        public string? MessageContent { get; set; }
        [Required]
        public string? MessageType { get; set; }
        public bool Viewed { get; set;}
        public string? FilePath { get; set; }

    }
}

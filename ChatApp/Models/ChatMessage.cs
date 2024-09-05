using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ChatId { get; set; }
        [Required]
        public int MessageId { get; set; }

    }
}

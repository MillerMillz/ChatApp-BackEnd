using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class RoomChatMessage
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public int RoomChatId { get; set; }
        [Required]
        public int MessageId { get; set; }
    }
}

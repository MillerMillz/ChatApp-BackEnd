using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class ChatRoomMembership
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RoomId { get; set; }
        [Required]
        public string UserId { get; set; }
        public DateTime Time { get; set; }
    }
}

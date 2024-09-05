using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class Roomchat
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string OwnerId { get; set; }
        [Required]
        public int RoomID { get; set; }
        [Required]
        public int LastMessageID { get; set; }
    }
}

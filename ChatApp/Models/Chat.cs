using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ownerId { get; set; }
        [Required]
        public string FriendId { get; set; }
        [Required]
        public int FriendshipId { get; set; }
        [Required]
        public int LastMessageID { get; set; }
    }
}

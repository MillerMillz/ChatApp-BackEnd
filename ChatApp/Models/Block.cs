using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class Block
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int FriendshipId { get; set; }
        [Required]
        public string BlockerId { get; set; }
    }
}

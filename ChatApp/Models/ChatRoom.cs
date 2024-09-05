using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class ChatRoom
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Bio { get; set; }
        [Required]
        public string AdminId { get; set; }
        public string? FilePath { get; set; }
    }
}

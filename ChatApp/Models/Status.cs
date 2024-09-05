using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }
        public string? Message { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime Time { get; set; }
        public string? FilePath { get; set; }
    }
}

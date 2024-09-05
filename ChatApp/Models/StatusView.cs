using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class StatusView
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StatusId { get; set; }
        [Required]
        public int ViewerId { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public string Shown { get; set; }   
    }
}

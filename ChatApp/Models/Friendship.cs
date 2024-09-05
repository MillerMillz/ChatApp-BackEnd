using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string User1ID { get; set; }
        [Required]
        public string User2ID { get; set; }
        [Required]
        public string Status { get; set; }
        public DateTime? Time { get; set; }
    }
}

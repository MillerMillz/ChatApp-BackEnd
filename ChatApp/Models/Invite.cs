using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class Invite
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SenderId { get; set; }
        [Required]
        public string ReciepientID { get; set; }
        public DateTime? Time { get; set; }
    }
}

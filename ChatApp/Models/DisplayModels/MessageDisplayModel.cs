using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models.DisplayModels
{
    public class MessageDisplayModel
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public ApplicationUser User { get; set; }
        [Required]
        public DateTime Time { get; set; }
        public int ReplyID { get; set; }
        public string? MessageContent { get; set; }
        [Required]
        public string? MessageType { get; set; }
        public bool Viewed { get; set; }
        public string? FilePath { get; set; }
        public string? TruePath { get; set; }
    }
}

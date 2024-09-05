using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class ApplicationUser:IdentityUser
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        public string Bio { get; set; }
        public string? FilePath { get; set; }
        public string? Image { get; set; }
    }
}

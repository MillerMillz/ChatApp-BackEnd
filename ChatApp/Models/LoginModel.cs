using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class LoginModel
    {
        public class Request
        {
            [Required]
            [EmailAddress]
            public string? Email { get; set; }
            [Required]
            public string? Password { get; set; }

        }
        public class Response
        {
            public ApplicationUser? User { get; set; }
            public IEnumerable<IdentityError> Errors { get; internal set; }
        }
    }
}

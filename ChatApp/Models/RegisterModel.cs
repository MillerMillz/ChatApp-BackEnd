using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class RegisterModel
    {
        public class Request
        {

            [Required]
            [EmailAddress]
            public string? Email { get; set; }
            [Required]
            public string? Password { get; set; }
            [MaxLength(50)]
            public string? FirstName { get; set; }
            [MaxLength(50)]
            public string? LastName { get; set; }
            public string? Bio { get; set; }
            public IFormFile? File { get; set; }
        }

        public class Response
        {
            public string? Email { get; set; }
            public IEnumerable<IdentityError>? Errors { get; internal set; }
        }
    }
}

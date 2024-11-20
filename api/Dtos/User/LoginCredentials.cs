using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class LoginCredentials
    {
        [Required]
        [MinLength(1, ErrorMessage = "The Email field is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(1, ErrorMessage = "The Password field is required.")]
        public string? Password { get; set; }
    }
}
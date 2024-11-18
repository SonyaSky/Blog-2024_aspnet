using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "The FullName field is required.")]
        [MaxLength(1000, ErrorMessage = " The field FullName must be a string or array type with a maximum length of '1000'.")]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [MinLength(6, ErrorMessage = "The field Password must be a string or array type with a minimum length of '6'.")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [MinLength(1, ErrorMessage = "The Email field is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; } = string.Empty;
    }
}
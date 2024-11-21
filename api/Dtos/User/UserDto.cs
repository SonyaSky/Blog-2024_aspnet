using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using api.Models;

namespace api.Dtos.User
{
    public class UserDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required] 
        public DateTime CreateTime{ get; set; } 
        [Required]
        [MinLength(1, ErrorMessage = "The FullName field is required.")]
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "The Email field is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Phone]
        public string? PhoneNumber { get; set; } = string.Empty;
    }
}
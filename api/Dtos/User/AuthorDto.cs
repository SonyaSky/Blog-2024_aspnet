using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Dtos
{
    public class AuthorDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "The FullName field is required.")]
        public string FullName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public int Posts { get; set; }
        public int Likes { get; set; }
        public DateTime Created { get; set; }
    }
}
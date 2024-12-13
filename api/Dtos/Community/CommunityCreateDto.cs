using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Community
{
    public class CommunityCreateDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "The Name field must be a string or array type with a minimum length of '5'.")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MinLength(5, ErrorMessage = "Description must be a string or array type with a minimum length of '5'.")]
        public string Description { get; set; } = string.Empty;
        [Required]
        public bool IsClosed { get; set; } = false;
    }
}
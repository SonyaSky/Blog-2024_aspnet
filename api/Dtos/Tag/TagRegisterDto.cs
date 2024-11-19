using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Tag
{
    public class TagRegisterDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "The Name field is required.")]
        public string Name { get; set; } = string.Empty;
    }
}
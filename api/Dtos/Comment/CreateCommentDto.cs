using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class CreateCommentDto
    {
        public Guid? ParentId { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "The Content field is required.")]
        [MaxLength(1000, ErrorMessage = "The field Content must be a string or array type with a maximum length of '1000'.")]
        public string Content { get; set; } = string.Empty;
    }
}
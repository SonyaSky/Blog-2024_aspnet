using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Validations;

namespace api.Dtos.Post
{
    public class CreatePostDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Incorrect post's title length")]
        [MaxLength(1000, ErrorMessage = " The field Title must be a string or array type with a maximum length of '1000'.")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(5, ErrorMessage = "Incorrect post's description length")]
        [MaxLength(5000, ErrorMessage = " The field Description must be a string or array type with a maximum length of '5000'.")]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Reading time must be a non-negative number.")]
        public int ReadingTime { get; set; }
        [UrlFormat(ErrorMessage = "The Image field is not a valid fully-qualified http, https, or ftp URL.")]
        public string? Image { get; set; } = string.Empty;
        public Guid? AddressId { get; set; }
        [Required]
        public List<Guid> Tags { get; set; }
    }
}
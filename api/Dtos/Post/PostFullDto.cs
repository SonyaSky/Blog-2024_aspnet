using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Post
{
    public class PostFullDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "The Title field is required.")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(1, ErrorMessage = "The Description field is required.")]
        public string Description { get; set; } = string.Empty;
        [Required]
        public int ReadingTime { get; set; }
        public string? Image { get; set; } = string.Empty;
        [Required]
        public Guid AuthorId { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "The Author field is required.")]
        public string Author { get; set; } = string.Empty;
        public Guid? CommunityId { get; set; }
        public string? CommunityName { get; set; } = string.Empty;
        public Guid? AddressId { get; set; }
        [Required]
        public int Likes { get; set; } = 0;
        [Required]
        public bool HasLike { get; set; } = false;
        [Required]
        public int CommentsCount { get; set; }
        public List<TagDto>? Tags { get; set; } = new List<TagDto>();
        [Required]
        public List<CommentDto>? Comments {get; set; } = new List<CommentDto>();
    }
}
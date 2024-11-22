using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Tag
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "The Name field is required.")]
        public string Name { get; set; } = string.Empty;
        public List<PostTag> PostTags { get; set; } = new List<PostTag>();

        public Tag()
        {
            Id = new Guid();
            CreateTime = DateTime.UtcNow;
        }
    }
}
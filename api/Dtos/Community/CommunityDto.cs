using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Community
{
    public class CommunityDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required] 
        public DateTime CreateTime{ get; set; } 
        [Required]
        [MinLength(1, ErrorMessage = "The Name field is required.")]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public bool IsClosed { get; set; } = false;
        [Required]
        public int SubscribersCount { get; set; } = 0;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ReadingTime { get; set; }
        public string? Image { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public string Author { get; set; } = string.Empty;
        public Guid CommunityId { get; set; }
        public string CommunityName { get; set; } = string.Empty;
        public Guid AddressId { get; set; }
        public int Likes { get; set; } = 0;
        public bool HasLike { get; set; } = false;
        public int CommentsCount { get; set; }
        public List<PostTag> PostTags { get; set; } = new List<PostTag>();
        //public List<Comment>? Comments {get; set; }
    }
}
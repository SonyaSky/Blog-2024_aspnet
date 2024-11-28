using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid PostId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Content { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int SubComments { get; set; }
        public Post Post { get; set; }

        public Comment()
        {
            Id = new Guid();
            ParentId = null;
            CreateTime = DateTime.UtcNow;
            ModifiedDate = null;
            DeleteDate = null;
            SubComments = 0;
        }
    }
}
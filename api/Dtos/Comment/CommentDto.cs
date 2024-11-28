using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Post
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Content { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid AuthorId { get; set; }
        public string Author { get; set; }
        public int SubComments { get; set; }
    }
}
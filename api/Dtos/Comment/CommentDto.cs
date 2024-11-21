using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Post
{
    public class CommentDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "The Content field is required.")]
        public string Content { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime DeleteDate { get; set; }
        [Required]
        public Guid AuthorId { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "The Author field is required.")]
        public string Author { get; set; }
        [Required]
        public int SubComments { get; set; }
    }


//     id*	string($uuid)
// createTime*	string($date-time)
// content*	string
// minLength: 1
// modifiedDate	string($date-time)
// nullable: true
// deleteDate	string($date-time)
// nullable: true
// authorId*	string($uuid)
// author*	string
// minLength: 1
// subComments*	integer($int32)
}
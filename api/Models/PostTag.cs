using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class PostTag
    {
        public Guid TagId { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        public Tag Tag { get; set; }
    }
}
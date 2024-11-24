using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Like
    {
        public Guid PostId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
    }
}
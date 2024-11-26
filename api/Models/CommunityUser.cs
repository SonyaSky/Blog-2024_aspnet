using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class CommunityUser
    {
        public string UserId { get; set; }
        public Guid CommunityId { get; set; }
        public CommunityRole CommunityRole {get; set; }
        public Community Community {get; set; }
        public User User { get; set; }
    }
}
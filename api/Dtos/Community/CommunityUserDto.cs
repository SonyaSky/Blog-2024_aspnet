using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Dtos.Community
{
    public class CommunityUserDto
    {
        public string UserId { get; set; }
        public Guid CommunityId { get; set; }
        public CommunityRole CommunityRole { get; set; }
    }
}
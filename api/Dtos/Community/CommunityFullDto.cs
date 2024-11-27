using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.User;

namespace api.Dtos.Community
{
    public class CommunityFullDto
    {
        public Guid Id { get; set; }
        public DateTime CreateTime{ get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsClosed { get; set; } = false;
        public int SubscribersCount { get; set; } = 0;
        public List<UserDto>? Administrators { get; set; } = new List<UserDto>();
    }
}
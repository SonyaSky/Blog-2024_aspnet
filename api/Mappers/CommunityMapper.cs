using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Community;
using api.Models;

namespace api.Mappers
{
    public static class CommunityMapper
    {
        public static CommunityDto ToCommunityDto(this Community community) 
        {
            return new CommunityDto{
                Id = community.Id,
                CreateTime = community.CreateTime,
                Name = community.Name,
                Description = community.Description,
                IsClosed = community.IsClosed,
                SubscribersCount = community.SubscribersCount
            };
        }
    }
}
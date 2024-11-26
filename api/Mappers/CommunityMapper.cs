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
        public static Community ToCommunityFromCreateDto(this CommunityCreateDto community) 
        {
            return new Community{
                Name = community.Name,
                Description = community.Description,
                IsClosed = community.IsClosed,
            };
        }

        public static CommunityUserDto ToCommunityUserDto(this CommunityUser cu) 
        {
            return new CommunityUserDto{
                UserId = cu.UserId,
                CommunityId = cu.CommunityId,
                CommunityRole = cu.CommunityRole == 0 ? CommunityRole.Administrator : CommunityRole.Subscriber
            };
        }
    }
}
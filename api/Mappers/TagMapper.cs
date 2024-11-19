using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Dtos.Tag;
using api.Models;

namespace api.Mappers
{
    public static class TagMapper
    {
        public static Tag ToTagFromRegisterDto(this TagRegisterDto tag) 
        {
            return new Tag{
                Name = tag.Name,
            };
        }

        public static TagDto ToTagDto(this Tag tag) 
        {
            return new TagDto{
                Id = tag.Id,
                CreateTime = tag.CreateTime,
                Name = tag.Name,
            };
        }
    }
}
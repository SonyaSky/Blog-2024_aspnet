using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Models;

namespace api.Mappers
{
    public static class AuthorMapper
    {
        public static AuthorDto ToAuthorDto(this Author author) 
        {
            return new AuthorDto{
                FullName = author.FullName,
                BirthDate = author.BirthDate,
                Gender = author.Gender,
                Posts = author.Posts,
                Likes = author.Likes,
                Created = author.Created
            };
        }
    }
}
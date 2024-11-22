using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Post;
using api.Models;

namespace api.Mappers
{
    public static class PostMapper
    {
        public static PostDto ToPostDto(this Post post)
        {
            return new PostDto
            {
                Id = post.Id,
                CreateTime = post.CreateTime,
                Title = post.Title,
                Description = post.Description,
                ReadingTime = post.ReadingTime,
                AuthorId = post.AuthorId,
                Author = post.Author,
                CommunityId = post.CommunityId,
                CommunityName = post.CommunityName,
                AddressId = post.AddressId,
                Likes = post.Likes,
                HasLike = post.HasLike,
                CommentsCount = post.CommentsCount
            };
        }
    }
}
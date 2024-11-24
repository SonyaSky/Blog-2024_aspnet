using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Post;
using api.Extensions;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers
{
    [Route("api/post")]
    [ApiController]
    public class PostController: ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;
        public PostController(ApplicationDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get list of available posts")]
        public IActionResult GetAll() 
        {
            var posts = _context.Posts.ToList();

            var postDtos = new List<PostDto>();
            foreach (var post in posts)
            {
                postDtos.Add(MakePost(post));
            }

            return Ok(postDtos);
        }

        private PostDto MakePost(Post post)
        {
            var newPost = post.ToPostDto();
            var tags = _context.PostTags
                .Where(t => t.PostId == post.Id)
                .Select(t => t.Tag.ToTagDto())
                .ToList();
            newPost.Tags = tags;
            return newPost;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePostDto createPostDto)
        {
           try 
           {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var username = User.GetUsername();
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return Unauthorized();
                }
                foreach (var tagId in createPostDto.Tags)
                {
                    var tag = await GetTagAsync(tagId); 
                    if (tag == null)
                    {
                        return BadRequest(
                            new Response
                            {
                                Status = "Error",
                                Message = $"Tag with Id='{tagId}' is not found."
                            }
                        );
                    }
                }
                var post = new Post{
                    Title = createPostDto.Title,
                    Description = createPostDto.Description,
                    ReadingTime = createPostDto.ReadingTime,
                    Image = createPostDto.Image,
                    AddressId = createPostDto.AddressId,
                    AuthorId = new Guid(user.Id),
                    Author = user.FullName,
                    CommunityId = null,
                    CommunityName = null
                };
                await _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync();
                foreach (var tagId in createPostDto.Tags)
                {
                    var postTagModel = new PostTag
                    {
                        PostId = post.Id,
                        TagId = tagId
                    };
                    await CreatePostTagAsync(postTagModel);
                    if (postTagModel == null)
                    {
                        return StatusCode(500, "Could not create post");
                    }
                }
                return Created();

           } 
           catch (Exception e) 
           {
                return StatusCode(500, e);
           }
        }

        private async Task<Tag?> GetTagAsync(Guid id)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
        }

        private async Task<PostTag> CreatePostTagAsync(PostTag postTag)
        {
            await _context.PostTags.AddAsync(postTag);
            await _context.SaveChangesAsync();
            return postTag;
        }

        private void MakeAuthor(User user)
        {
            var author = _context.Authors.FirstOrDefault(a => a.UserId == user.Id);
            if (author != null)
            {
                author.Posts += 1;
            }
            else
            {
                var newAuthor = new Author
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    BirthDate = user.BirthDate,
                    Gender = user.Gender,
                    Posts = 1,
                    Likes = 0,
                    Created = user.CreateTime
                };
                _context.Authors.Add(newAuthor);
                _context.SaveChanges();
            }
        }
    }
}

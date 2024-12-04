using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Post;
using api.Extensions;
using api.Mappers;
using api.Models;
using api.Queries;
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
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query) 
        {
            if (query.Tags.Count != 0)
            {
                foreach (var id in query.Tags)
                {
                    var tag = await GetTagAsync(id);
                    if (tag == null)
                    {
                        return BadRequest(
                            new Response
                            {
                                Status = "Error",
                                Message = $"Tag with id={id} was not found in the database"
                            }
                        );
                    }
                }
            }
            var posts = await GetPostsAsync(query);
            var postDtos = new PostPagedListDto();
            foreach (var post in posts.Posts)
            {
                var newPost = MakePost(post);
                newPost.HasLike = await CheckForLike(post.Id);
                postDtos.Posts.Add(newPost);
            }
            postDtos.Pagination = posts.Pagination;

            return Ok(postDtos);
        }
        private async Task<PostPagedList> GetPostsAsync(QueryObject query)
        {
            var posts = _context.Posts.AsQueryable();
            var postPagedList = new PostPagedList();
            postPagedList.Pagination.Size = query.Size;
            postPagedList.Pagination.Count = 0;
            postPagedList.Pagination.Current = query.Page;
            if (query.Tags.Count != 0)
            {
                var postIdsWithTags = await _context.PostTags
                    .Where(pt => query.Tags.Contains(pt.TagId))
                    .Select(pt => pt.PostId)
                    .Distinct()
                    .ToListAsync();

                posts = posts.Where(p => postIdsWithTags.Contains(p.Id));
            }
            if (!string.IsNullOrWhiteSpace(query.Author))
            {
                posts = posts.Where(p => p.Author.Contains(query.Author));
            }
            if (query.Min.HasValue)
            {
                posts = posts.Where(p => p.ReadingTime >= query.Min);
            }
            if (query.Max.HasValue)
            {
                posts = posts.Where(p => p.ReadingTime <= query.Max);
            }
            if (User.Identity.IsAuthenticated)
            {
                var username = User.GetUsername();
                var user = await _userManager.FindByNameAsync(username);
                var allowed = new List<Guid>();
                if (user == null)
                {
                    allowed = await GetAllowedCommunities();
                }
                else
                {
                    allowed = await GetAllowedCommunities(user.Id);
                }
                posts = posts.Where(p => p.CommunityId == null || allowed.Contains(p.CommunityId.Value));
                if (query.OnlyMyCommunities.HasValue)
                {
                    if (query.OnlyMyCommunities == true && user != null)
                    {
                        var userCommunities = await CommunitiesAsync(user.Id);
                        if (userCommunities == null)
                        {
                            return postPagedList;
                        }
                        posts = posts.Where(p => p.CommunityId.HasValue && userCommunities.Contains(p.CommunityId.Value));
                    }
                }
            } else
            {
                var allowed = await GetAllowedCommunities();
                posts = posts.Where(p => p.CommunityId == null || allowed.Contains(p.CommunityId.Value));
            }
            if (query.Sorting.HasValue)
            {
                if (query.Sorting == PostSorting.LikeAsc) 
                {
                    posts = posts.OrderBy(p => p.Likes);
                }
                if (query.Sorting == PostSorting.LikeDesc) 
                {
                    posts = posts.OrderByDescending(p => p.Likes);
                }
                if (query.Sorting == PostSorting.CreateAsc) 
                {
                    posts = posts.OrderBy(p => p.CreateTime);
                }
                if (query.Sorting == PostSorting.CreateDesc) 
                {
                    posts = posts.OrderByDescending(p => p.CreateTime);
                }
            }
            var count = await posts.CountAsync();
            var skipNumber = (query.Page - 1) * query.Size;
            postPagedList.Posts = await posts.Skip(skipNumber).Take(query.Size).ToListAsync();
            postPagedList.Pagination.Count = (count + query.Size - 1) / query.Size;
            return postPagedList;
        }

        private async Task<List<Guid>> CommunitiesAsync(string userId)
        {
            return await _context.CommunityUsers
                .Where(cu => cu.UserId == userId)
                .Select(cu => cu.CommunityId)
                .ToListAsync();
        }

        private async Task<List<Guid>> GetAllowedCommunities(string? id = null)
        {
            var allowed = await _context.Communities
                .Where(c => c.IsClosed == false)
                .Select(c => c.Id)
                .ToListAsync();
            if (id == null)
            {
                return allowed;
            }
            else 
            {
                var cu = await _context.CommunityUsers
                    .Where(c => c.UserId == id)
                    .Select(c => c.CommunityId)
                    .ToListAsync();
                return allowed.Union(cu).ToList();
            }
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
        [SwaggerOperation(Summary = "Create a personal user post")]
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
                MakeAuthor(user);
                return Ok(post.Id);

           } 
           catch (Exception e) 
           {
                return StatusCode(500, e);
           }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get information about a specific post")]
        public async Task<IActionResult> GetOnePost([FromRoute] Guid id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Post with id={id} was not found in the database"
                    }
                );
            }
            var newPost = post.ToPostFullDto();
            var tags = await _context.PostTags
                .Where(t => t.PostId == id)
                .Select(t => t.Tag.ToTagDto())
                .ToListAsync();
            newPost.Tags = tags;
            var comments = await _context.Comments
                .Where(c => c.PostId == id && c.ParentId == null)
                .Select(c => c.ToCommentDto())
                .ToListAsync();
            newPost.Comments = comments;
            newPost.HasLike = await CheckForLike(post.Id);
            return Ok(newPost);
        }


        [HttpPost("{id}/like")]
        [Authorize]
        [SwaggerOperation(Summary = "Add like to a specific post")]
        public async Task<IActionResult> AddLike([FromRoute] Guid id)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            var post = _context.Posts.Find(id);
            if (post == null)
            {
                return NotFound(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Post with id={id} was not found in the database"
                    }
                );
            }
            var like = _context.Likes.FirstOrDefault(l => l.PostId == id && l.UserId == user.Id);
            if (like != null)
            {
                return BadRequest(
                    new Response
                    {
                        Status = "Error",
                        Message = "Like on this post is already set by this user"
                    }
                );
            }
            var likeModel = new Like
            {
                PostId = id,
                UserId = user.Id
            };
            var author = await GetAuthorAsync(post.AuthorId.ToString());
            if (author == null)
            {
                return BadRequest(
                    new Response
                    {
                        Status = "Error",
                        Message = "Couldn't find author of the post in the database"
                    }
                );
            }
            author.Likes += 1;
            post.Likes += 1;
            await CreateLikeAsync(likeModel);
            if (likeModel == null)
            {
                return StatusCode(500, "Could not create post");
            }
            return Created();

        }

        [HttpDelete("{id}/like")]
        [Authorize]
        [SwaggerOperation(Summary = "Delete like from a specific post")]
        public async Task<IActionResult> DeleteLike([FromRoute] Guid id)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            var post = _context.Posts.Find(id);
            if (post == null)
            {
                return NotFound(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Post with id={id} was not found in the database"
                    }
                );
            }
            var like = _context.Likes.FirstOrDefault(l => l.PostId == id && l.UserId == user.Id);
            if (like == null)
            {
                return BadRequest(
                    new Response
                    {
                        Status = "Error",
                        Message = "Like on this post is not yet set by this user"
                    }
                );
            }
            var author = await GetAuthorAsync(post.AuthorId.ToString());
            if (author == null)
            {
                return BadRequest(
                    new Response
                    {
                        Status = "Error",
                        Message = "Couldn't find author of the post in the database"
                    }
                );
            }
            author.Likes -= 1;
            post.Likes -= 1;
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
            return Ok();

        }


        private async Task<Like> CreateLikeAsync(Like like)
        {
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
            return like;
        }

        private async Task<bool> CheckForLike(Guid postId)
        {

            if (User.Identity.IsAuthenticated)
            {
                var username = User.GetUsername();
                if (!string.IsNullOrEmpty(username))
                {
                    var user = await _userManager.FindByNameAsync(username);
                    if (user != null)
                    {
                        var like = _context.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == user.Id);
                        if (like != null)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        private async Task<Tag?> GetTagAsync(Guid id)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
        }

        private async Task<Author?> GetAuthorAsync(string id)
        {
            return await _context.Authors.FirstOrDefaultAsync(a => a.UserId == id);
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
            }
            _context.SaveChanges();
        }
    }
}

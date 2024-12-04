using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Community;
using api.Dtos.Post;
using api.Extensions;
using api.Interfaces;
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

    [Route("api/community")]
    [ApiController]
    
    public class CommunityController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        public CommunityController(ApplicationDBContext context, UserManager<User> userManager, ITokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get community list")]
        [AllowAnonymous]
        public IActionResult GetAll() 
        {
            var communities = _context.Communities
            .Select(c => c.ToCommunityDto()).ToList();
            return Ok(communities);
        }

        [HttpGet("my")]
        [Authorize]
        [SwaggerOperation(Summary = "Get user's community list (with user's role in the community)")]
        public async Task<IActionResult> GetUserCommunityList()
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var isValidToken = await _tokenService.IsTokenValid(token);
            if (!isValidToken)
            {
                return Unauthorized();
            }
            var communityUsers = await _context.CommunityUsers
                .Where(c => c.UserId == user.Id)
                .Select(c => c.ToCommunityUserDto())
                .ToListAsync();
            return Ok(communityUsers);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get information about community")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var community = _context.Communities.Find(id);
            if (community == null) {
                return NotFound(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Community with id={id} was not found in the database"
                    }
                );
            }
            var communityFullDto = community.ToCommunityFullDto();
            var admins = await _context.CommunityUsers
                .Where(a => a.CommunityId == id && a.CommunityRole == CommunityRole.Administrator)
                .ToListAsync();
            if (admins != null)
            {
                admins.ForEach(a => {
                    var admin = _userManager.Users.FirstOrDefault(u => u.Id == a.UserId);
                    if (admin != null){
                        communityFullDto.Administrators.Add(admin.ToUserDto());
                    }
                });
            }
            return Ok(communityFullDto);
        }

        [HttpGet("{id}/post")]
        [SwaggerOperation(Summary = "Get community's posts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPosts([FromRoute] Guid id, [FromQuery] SmallQueryObject query)
        {
            var community = _context.Communities.Find(id);
            if (community == null) {
                return NotFound(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Community with id={id} was not found in the database"
                    }
                );
            }
            if (community.IsClosed == true)
            {
                User? user = null;
                if (User.Identity.IsAuthenticated)
                {
                    var username = User.GetUsername();
                    user = await _userManager.FindByNameAsync(username);
                }
                if (user == null)
                {
                    return StatusCode(403, new Response
                    { 
                        Status = "Error", 
                        Message = $"Access to closed community with id={id} is forbidden." 
                    }
                    );
                }
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var isValidToken = await _tokenService.IsTokenValid(token);
                if (!isValidToken)
                {
                    return StatusCode(403, new Response
                    { 
                        Status = "Error", 
                        Message = $"Access to closed community with id={id} is forbidden." 
                    }
                    );
                }
                var cu = await _context.CommunityUsers.FirstOrDefaultAsync(c => c.CommunityId == id && c.UserId == user.Id);
                if (cu == null)
                {
                    return StatusCode(403, new Response
                    { 
                        Status = "Error", 
                        Message = $"Access to closed community with id={id} is forbidden." 
                    }
                    );
                }
            }
            if (query.Tags.Count != 0)
            {
                foreach (var tagId in query.Tags)
                {
                    var tag = await GetTagAsync(tagId);
                    if (tag == null)
                    {
                        return BadRequest(
                            new Response
                            {
                                Status = "Error",
                                Message = $"Tag with id={tagId} was not found in the database"
                            }
                        );
                    }
                }
            }
            var posts = await GetPostsAsync(query, id);
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

        private async Task<PostPagedList> GetPostsAsync(SmallQueryObject query, Guid id)
        {
            var posts = _context.Posts
                .Where(p => p.CommunityId == id)
                .AsQueryable();
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

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Create new community")]
        public async Task<IActionResult> Create([FromBody] CommunityCreateDto communityCreateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var isValidToken = await _tokenService.IsTokenValid(token);
            if (!isValidToken)
            {
                return Unauthorized();
            }
            var communityModel = communityCreateDto.ToCommunityFromCreateDto();
            _context.Communities.Add(communityModel);
            var communityUserModel = new CommunityUser
            {
                UserId = user.Id,
                CommunityId = communityModel.Id,
                CommunityRole = CommunityRole.Administrator
            };
            _context.CommunityUsers.Add(communityUserModel);
            _context.SaveChanges();
            return Ok(communityModel.Id);
        }

        [HttpPost("{id}/post")]
        [Authorize]
        [SwaggerOperation(Summary = "Create a post in the specified community")]
        public async Task<IActionResult> Create([FromRoute] Guid id, [FromBody] CreatePostDto createPostDto)
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
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var isValidToken = await _tokenService.IsTokenValid(token);
                if (!isValidToken)
                {
                    return Unauthorized();
                }
                var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == id);
                if (community == null)
                {
                    return NotFound(
                        new Response
                        {
                            Status = "Error",
                            Message = $"Community with id={id} was not found in the database"
                        }
                    );
                }
                var admin = await _context.CommunityUsers.FirstOrDefaultAsync(c => c.CommunityId == id && c.UserId == user.Id && c.CommunityRole == CommunityRole.Administrator);
                if (admin == null)
                {
                    return StatusCode(403, new Response
                    { 
                        Status = "Error", 
                        Message = $"User with id={user.Id} is not able to post in community with id={id}." 
                    }
                    );
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
                    CommunityId = id,
                    CommunityName = community.Name
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

        [HttpGet("{id}/role")]
        [Authorize]
        [SwaggerOperation(Summary = "Get user's greatest role in the community (or null if the user is not a member of the community)")]
        public async Task<IActionResult> GetRole([FromRoute] Guid id)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var isValidToken = await _tokenService.IsTokenValid(token);
            if (!isValidToken)
            {
                return Unauthorized();
            }
            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == id);
            if (community == null)
            {
                return NotFound(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Community with id={id} was not found in the database"
                    }
                );
            }
            var admin = await _context.CommunityUsers.FirstOrDefaultAsync(c => c.CommunityId == id && c.UserId == user.Id && c.CommunityRole == CommunityRole.Administrator);
            if (admin != null)
            {
                return Ok("Administrator");
            }
            var subscriber = await _context.CommunityUsers.FirstOrDefaultAsync(c => c.CommunityId == id && c.UserId == user.Id && c.CommunityRole == CommunityRole.Subscriber);
            if (subscriber != null)
            {
                return Ok("Subscriber");
            }
            return Ok("null");
        }


        [HttpPost("{id}/subscribe")]
        [Authorize]
        [SwaggerOperation(Summary = "Subscribe user to the community")]
        public async Task<IActionResult> Subscribe([FromRoute] Guid id)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var isValidToken = await _tokenService.IsTokenValid(token);
            if (!isValidToken)
            {
                return Unauthorized();
            }
            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == id);
            if (community == null)
            {
                return NotFound(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Community with id={id} was not found in the database"
                    }
                );
            }
            //сейчас всем можно подписаться
            var cu = _context.CommunityUsers.FirstOrDefault(c => c.CommunityId == id && c.UserId == user.Id && c.CommunityRole == CommunityRole.Subscriber);
            if (cu != null)
            {
                return BadRequest(
                    new Response
                    {
                        Status = "Error",
                        Message = $"User with id={user.Id} is already subscribed to the community with id={id}"
                    }
                );
            }
            var communityUserModel = new CommunityUser
            {
                UserId = user.Id,
                CommunityId = id,
                CommunityRole = CommunityRole.Subscriber
            };
            community.SubscribersCount += 1;
            _context.CommunityUsers.Add(communityUserModel);
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}/unsubscribe")]
        [Authorize]
        [SwaggerOperation(Summary = "Unsubscribe user from the community")]
        public async Task<IActionResult> Unsubscribe([FromRoute] Guid id)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var isValidToken = await _tokenService.IsTokenValid(token);
            if (!isValidToken)
            {
                return Unauthorized();
            }
            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == id);
            if (community == null)
            {
                return NotFound(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Community with id={id} was not found in the database"
                    }
                );
            }

            var cu = _context.CommunityUsers.FirstOrDefault(c => c.CommunityId == id && c.UserId == user.Id && c.CommunityRole == CommunityRole.Subscriber);
            if (cu == null)
            {
                return BadRequest(
                    new Response
                    {
                        Status = "Error",
                        Message = $"User with id={user.Id} is not yet subscribed to the community with id={id}"
                    }
                );
            }
            community.SubscribersCount -= 1;
            _context.CommunityUsers.Remove(cu);
            _context.SaveChanges();
            return Ok();
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

        private async Task<bool> CheckForLike(Guid postId)
        {

            if (User .Identity.IsAuthenticated)
            {
                var username = User.GetUsername();
                if (!string.IsNullOrEmpty(username))
                {
                    var user = await _userManager.FindByNameAsync(username);
                    if (user != null)
                    {
                        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                        var isValidToken = await _tokenService.IsTokenValid(token);
                        if (isValidToken)
                        {
                            var like = await _context.Likes
                                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == user.Id);
                            return like != null;
                        }
                    }
                }
            }
            return false;
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
    }
}
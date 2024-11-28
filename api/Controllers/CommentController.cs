using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Extensions;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CommentController: ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;
        public CommentController(ApplicationDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("comment")]
        [SwaggerOperation(Summary = "Get ")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() 
        {
            var comments = await _context.Comments.ToListAsync();

            return Ok(comments);
        }

        [HttpPost("post/{id}/comment")]
        [SwaggerOperation(Summary = "Add comment to a specific post")]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromRoute] Guid id, [FromBody] CreateCommentDto createCommentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
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
            if (post.CommunityId != null)
            {
                var cu = _context.CommunityUsers.FirstOrDefault(c => c.CommunityId == post.CommunityId && c.UserId == user.Id);
                if (cu == null)
                {
                    return StatusCode(403, new Response
                        {
                            Status = "Error",
                            Message = $"Access to closed community post with id={id} is forbidden for user Id={user.Id}"
                        }
                    );
                }
            }
            if (createCommentDto.ParentId != null)
            {
                var parent = await _context.Comments.FirstOrDefaultAsync(c => c.Id == createCommentDto.ParentId);
                if (parent == null)
                {
                    return NotFound(
                        new Response
                        {
                            Status = "Error",
                            Message = $"Comment with id={createCommentDto.ParentId}  was not found in the database"
                        }
                    );
                }
                parent.SubComments += 1;
            }
            var newComment = new Comment
            {
                ParentId = createCommentDto.ParentId,
                PostId = id,
                Content = createCommentDto.Content,
                AuthorId = new Guid(user.Id),
                AuthorName = user.FullName
            };
            post.CommentsCount += 1;
            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();
            return Created();
        }
    }
}
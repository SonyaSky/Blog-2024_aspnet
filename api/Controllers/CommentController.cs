using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
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

        [HttpGet("comment/all")]
        [SwaggerOperation(Summary = "Get ")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() 
        {
            var comments = await _context.Comments.ToListAsync();

            return Ok(comments);
        }

        [HttpGet("comment/{id}/tree")]
        [SwaggerOperation(Summary = "Get all nested comments (replies)")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTree([FromRoute] Guid id) 
        {
            var parent = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (parent == null)
            {
                return NotFound(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Comment with id={id} was not found in the database"
                    }
                );
            }
            if (parent.RootId != null)
            {
                return BadRequest(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Comment with id={id} is not a root element"
                    }
                );
            }
            var comments = await _context.Comments
                .Where(c => c.RootId == id)
                .Select(c => c.ToCommentDto())
                .ToListAsync();

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
            Guid? root = null;
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
                if (parent.RootId == null)
                {
                    root = parent.Id;
                }
                else
                {
                    root = parent.RootId;
                }
            }
            var newComment = new Comment
            {
                ParentId = createCommentDto.ParentId,
                PostId = id,
                Content = createCommentDto.Content,
                AuthorId = new Guid(user.Id),
                AuthorName = user.FullName,
                RootId = root
            };
            post.CommentsCount += 1;
            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();
            return Created();
        }

        [HttpPut("comment/{id}")]
        [SwaggerOperation(Summary = "Edit a specific comment")]
        [Authorize]
        public async Task<IActionResult> EditComment([FromRoute] Guid id, [FromBody] CommentEditDto commentEditDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return NotFound(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Comment with id={id} not found in database"
                    }
                );
            }
            if (new Guid(user.Id) != comment.AuthorId)
            {
                return StatusCode(403, new Response
                    {
                        Status = "Error",
                        Message = $"The user with id={user.Id} is not the author of this comment"
                    }
                );
            }
            if (comment.DeleteDate != null)
            {
                return BadRequest(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Comment with id={id} is deleted"
                    }
                );
            }
            comment.Content = commentEditDto.Content;
            comment.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("comment/{id}")]
        [SwaggerOperation(Summary = "Delete a specific comment")]
        [Authorize]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid id)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return NotFound(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Comment with id={id} not found in database"
                    }
                );
            }
            if (new Guid(user.Id) != comment.AuthorId)
            {
                return StatusCode(403, new Response
                    {
                        Status = "Error",
                        Message = $"The user with id={user.Id} is not the author of this comment"
                    }
                );
            }
            if (comment.DeleteDate != null)
            {
                return BadRequest(
                    new Response
                    {
                        Status = "Error",
                        Message = $"Comment with id={id} is already deleted"
                    }
                );
                //или можно вернуть OK() как в сваггере
            }
            if (comment.SubComments != 0)
            {
                comment.ModifiedDate = DateTime.UtcNow;
                comment.DeleteDate = DateTime.UtcNow;
                comment.Content = "";
            } else
            {
                if (comment.ParentId != null)
                {
                    var parent = await _context.Comments.FindAsync(comment.ParentId);
                    if (parent != null)
                    {
                        parent.SubComments -= 1;
                    }
                }
                var post = await _context.Posts.FindAsync(comment.PostId);
                if (post != null)
                {
                    post.CommentsCount -=1;
                }
                _context.Comments.Remove(comment);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Community;
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

    [Route("api/community")]
    [ApiController]
    
    public class CommunityController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;
        public CommunityController(ApplicationDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get information about community")]
        [AllowAnonymous]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var community = _context.Communities.Find(id);
            if (community == null) {
                return NotFound();
            }
            return Ok(community);
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
            return Created();
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
            var communityUsers = await _context.CommunityUsers
                .Where(c => c.UserId == user.Id)
                .Select(c => c.ToCommunityUserDto())
                .ToListAsync();
            return Ok(communityUsers);
        }

        [HttpPost("{id}/subscribe")]
        [Authorize]
        [SwaggerOperation(Summary = "Subscribe a user to the community")]
        public async Task<IActionResult> Subscribe([FromRoute] Guid id)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
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
    }
}
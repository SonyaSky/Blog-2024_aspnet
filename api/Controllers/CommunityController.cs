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
                CommunityId = communityModel.Id
            };
            _context.CommunityUsers.Add(communityUserModel);
            _context.SaveChanges();
            return Created();
        }
    }
}
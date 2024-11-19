using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Community;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers
{

    [Route("api/community")]
    [ApiController]
    
    public class CommunityController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public CommunityController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get community list")]
        public IActionResult GetAll() 
        {
            var communities = _context.Communities
            .Select(c => c.ToCommunityDto()).ToList();
            return Ok(communities);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get information about community")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var community = _context.Communities.Find(id);
            if (community == null) {
                return NotFound();
            }
            return Ok(community);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CommunityCreateDto communityCreateDto)
        {
            var communityModel = communityCreateDto.ToCommunityFromCreateDto();
            _context.Communities.Add(communityModel);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = communityModel.Id }, communityModel);
        }
    }
}
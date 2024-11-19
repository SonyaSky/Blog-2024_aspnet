using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Tag;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers
{
    [Route("api/tag")]
    [ApiController]
    
    public class TagController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public TagController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get tag list")]
        public IActionResult GetAll() 
        {
            var tags = _context.Tags
            .Select(t => t.ToTagDto()).ToList();
            return Ok(tags);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var tag = _context.Tags.Find(id);
            if (tag == null) {
                return NotFound();
            }
            return Ok(tag);
        }

        [HttpPost]
        public IActionResult Create([FromBody] TagRegisterDto tag)
        {
            var tagModel = tag.ToTagFromRegisterDto();
            _context.Tags.Add(tagModel);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = tagModel.Id }, tagModel);
        }
    }
}
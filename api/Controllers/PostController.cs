using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers
{
    public class PostController: ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public PostController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get list of available posts")]
        public IActionResult GetAll() 
        {
            var posts = _context.Posts
            .Select(p => p.ToPostDto()).ToList();
            return Ok(posts);
        }
    }
}
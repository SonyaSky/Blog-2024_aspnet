using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers
{
    [Route("api/author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public AuthorController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("list")]
        [AllowAnonymous]
        public IActionResult GetAll() 
        {
            var authors = _context.Authors
            .Select(t => t.ToAuthorDto()).ToList();
            return Ok(authors);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Mappers;
using api.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/adress")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public AddressController(ApplicationDBContext context)
        {
            _context = context;
        }
        
        [HttpGet("address")]
        [AllowAnonymous]
        public IActionResult GetAddress()
        {
            var address = _context.AddressElements.AsQueryable();
            return Ok(address.Take(10).ToList());
        }

        [HttpGet("hier")]
        [AllowAnonymous]
        public IActionResult GetHier()
        {
            var address = _context.Hierarchies.AsQueryable();
            return Ok(address.Take(10).ToList());
        }

        [HttpGet("house")]
        [AllowAnonymous]
        public IActionResult GetHouse()
        {
            var address = _context.Houses.AsQueryable();
            return Ok(address.Take(10).ToList());
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string? query, [FromQuery] int parentObjectId = 0)
        {
            Console.WriteLine($"ParentObjectId: {parentObjectId}, Query: {query}");
            var hierarchies = await _context.Hierarchies
                .Where(h => h.ParentObjId == parentObjectId)
                .Select(h => h.ObjectId)
                .ToListAsync();
            var addresses = _context.AddressElements.AsQueryable();
            addresses = addresses.Where(a => hierarchies.Contains(a.ObjectId));
            if (!string.IsNullOrWhiteSpace(query))
            {
                addresses = addresses.Where(a => a.Name.Contains(query));
            }
            var newAddresses = addresses.Take(10).Select(a => a.ToAddressDto());
            return Ok(newAddresses);
        }

    }
}
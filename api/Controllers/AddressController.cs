using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
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
        public async Task<IActionResult> GetAddress()
        {
            var address = _context.AddressElements.AsQueryable();;
            return Ok(address.Take(10).ToList());
        }

        [HttpGet("hier")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHier()
        {
            var address = _context.Hierarchies.AsQueryable();;
            return Ok(address.Take(10).ToList());
        }

        [HttpGet("house")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHouse()
        {
            var address = _context.Houses.AsQueryable();;
            return Ok(address.Take(10).ToList());
        }

    }
}
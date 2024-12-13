using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Address;
using api.Mappers;
using api.Models;
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
    
        //удалила неактуальные версии адресов и домов
        // [HttpGet("address")]
        // [AllowAnonymous]
        // public async Task<IActionResult> GetAddress()
        // {
        //     var address = await _context.AddressElements
        //         .Where(h => h.NextId != 0)
        //         .ToListAsync();
        //     foreach (var a in address)
        //     {
        //         _context.AddressElements.Remove(a);
        //     }
        //     await _context.SaveChangesAsync();
        //     return Ok(address.Count()); 
        // }


        // [HttpGet("house")]
        // [AllowAnonymous]
        // public async Task<IActionResult> GetHouse()
        // {
        //     var address = await _context.Houses
        //         .Where(h => h.NextId != 0)
        //         .ToListAsync();
        //     foreach (var h in address)
        //     {
        //         _context.Houses.Remove(h);
        //     }
        //     await _context.SaveChangesAsync();
        //     return Ok();
        // }

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
            var houses = _context.Houses.AsQueryable();
            houses = houses.Where(h => hierarchies.Contains(h.ObjectId));
            if (!string.IsNullOrWhiteSpace(query))
            {
                addresses = addresses.Where(a => a.Name.Contains(query));
            }
            if (!string.IsNullOrWhiteSpace(query))
            {
                var houses0 = houses.Where(a => a.HouseNum == query);
                var houses1 = houses.Where(a => a.HouseNum.StartsWith(query)).Take(10);
                var houses2 = houses.Where(a => a.HouseNum.Contains(query)).Take(10);
                var combinedHouses = houses0
                    .Concat(houses1)
                    .Concat(houses2)
                    .Distinct()
                    .OrderBy(h => h.HouseNum);
                foreach (var h in combinedHouses)
                {
                    Console.WriteLine(h.HouseNum);
                }
                houses = combinedHouses;
            }
            var result1 = addresses.Take(10).Select(a => a.ToAddressDtoFromAddress()).ToList();
            var result2 = houses.Take(10).Select(a => a.ToAddressDtoFromHouse()).ToList();
            return Ok(result1.Concat(result2).Take(10));
        }

        [HttpGet("chain")]
        [AllowAnonymous]
        public async Task<IActionResult> GetChain([FromQuery] Guid objectGuid)
        {
            var id = FindElement(objectGuid.ToString());
            if (id == null)
            {
                return StatusCode(500, new Response
                {
                    Status = "Error occured",
                    Message = $"Object with ObjectGuid = {objectGuid} was not found"
                });
            }
            var hierarchies = await _context.Hierarchies.Where(h => h.ObjectId == id).ToListAsync();
            if (hierarchies == null)
            {
                return StatusCode(500, new Response
                {
                    Status = "Error occured",
                    Message = $"Object with ObjectGuid = {objectGuid} was not found"
                });
            }
            var hierarchy = new Hierarchy();
            if (hierarchies.Count() > 1)
            {
                hierarchy = hierarchies.FirstOrDefault(h => h.IsActive);
                if (hierarchy == null)
                {
                    return StatusCode(500, new Response
                    {
                        Status = "Error occured",
                        Message = $"Object with ObjectGuid = {objectGuid} was not found"
                    });
                }
            }
            else 
            {
                hierarchy = hierarchies[0];
            }
            
            int[] path = Array.ConvertAll(hierarchy.Path.Split('.'), int.Parse);
            var result = new List<SearchAddressModel>();
            foreach (var el in path)
            {
                result.Add(FindElementById(el));
            }
            return Ok(result);
        }

        private int? FindElement(string objectGuid)
        {
            var address = _context.AddressElements.FirstOrDefault(a => a.ObjectGuid == objectGuid);
            if (address != null)
            {
                return address.ObjectId;
            }
            var house = _context.Houses.FirstOrDefault(a => a.ObjectGuid == objectGuid);
            if (house != null)
            {
                return house.ObjectId;
            }
            return null;
        }

        private SearchAddressModel? FindElementById(int id)
        {
            var address = _context.AddressElements.FirstOrDefault(a => a.ObjectId == id);
            if (address != null)
            {
                return address.ToAddressDtoFromAddress();
            }
            var house = _context.Houses.FirstOrDefault(a => a.ObjectId == id);
            if (house != null)
            {
                return house.ToAddressDtoFromHouse();
            }
            return null;
        }
    }
}
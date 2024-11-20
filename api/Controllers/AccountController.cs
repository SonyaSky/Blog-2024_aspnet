using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        [ActivatorUtilitiesConstructor]
        public AccountController(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto) {
            try 
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = new User 
                {
                    UserName = registerDto.FullName,
                    FullName = registerDto.FullName,
                    Password = registerDto.Password,
                    Email = registerDto.Email,
                    BirthDate = registerDto.BirthDate,
                    Gender = registerDto.Gender, 
                    PhoneNumber = registerDto.PhoneNumber
                };
                var createdUser = await _userManager.CreateAsync(user, registerDto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new TokenResponse
                            {
                                Token = _tokenService.CreateToken(user)
                            }
                        );
                    } 
                    else 
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                } 
                else 
                {
                    return StatusCode(500, createdUser.Errors);
                }

            } catch (Exception e) 
            {
                return StatusCode(500, e);
            }
        }



        // [HttpGet]
        // [SwaggerOperation(Summary = "Get tag list")]
        // public IActionResult GetAll() 
        // {
        //     var tags = _context.Tags
        //     .Select(t => t.ToTagDto()).ToList();
        //     return Ok(tags);
        // }
// {
//   "fullName": "SecondUser",
//   "password": "P@ssword2",
//   "email": "user2@example.com",
//   "birthDate": "2024-11-20T08:12:15.707Z",
//   "gender": "Female",
//   "phoneNumber": "+7 (222) 222-22-22"
// }
    }
}
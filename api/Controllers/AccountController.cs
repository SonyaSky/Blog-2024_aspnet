using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Data;
using api.Dtos;
using api.Dtos.User;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        [ActivatorUtilitiesConstructorAttribute]
        public AccountController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto) {
            try 
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var existingUser  = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser  != null)
                {
                    return BadRequest($"Email '{registerDto.Email}' is already taken.");
                }

                var user = new User 
                {
                    UserName = registerDto.Email,
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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginCredentials loginCredentials)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginCredentials.Email);
            if (user == null) {
                return Unauthorized(
                    new Response
                    {
                        Status = null,
                        Message = "Login failed"
                    }
                );
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginCredentials.Password, false);
            if (!result.Succeeded) 
            {
                return Unauthorized(
                    new Response
                    {
                        Status = null,
                        Message = "Login failed"
                    }
                );
            }

            return Ok(
                new TokenResponse
                {
                    Token = _tokenService.CreateToken(user)
                }
            );
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Console.WriteLine(ClaimTypes.NameIdentifier);
            // if (userId == null)
            // {
            //     return NotFound("User  not found.");
            // }

            // // Find the user by ID
            // var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // if (userId == null)
            // {
            //     return Unauthorized();
            // }

            // var user = await _userManager.FindByIdAsync(userId);
            // if (user == null)
            // {
            //     return Unauthorized();
            // }


            return Ok(user.ToUserDto());
        } 

    }
}
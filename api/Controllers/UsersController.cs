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
    
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        [ActivatorUtilitiesConstructorAttribute]
        public UsersController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register new user")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto) {
            try 
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var existingUser  = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser  != null)
                {
                    return BadRequest(
                        new Response
                        {
                            Status = "Error",
                            Message = $"Email '{registerDto.Email}' is already taken."
                        }
                    );
                }

                var user = registerDto.ToUserFromRegisterDto();
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
        [SwaggerOperation(Summary = "Login to the system")]
        public async Task<IActionResult> Login(LoginCredentials loginCredentials)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginCredentials.Email);
            if (user == null) {
                return BadRequest(
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
                return BadRequest(
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
        [SwaggerOperation(Summary = "Get user profile")]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(user.ToUserDto());
        } 

        [HttpPost("logout")]
        [Authorize]
        [SwaggerOperation(Summary = "Log out from the system")]
        public async Task<IActionResult> Logout()
        {
            // Check if the user is authenticated
            if (!User .Identity.IsAuthenticated)
            {
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User  is not authenticated."
                });
            }

            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            
            // Check if the user exists
            if (user == null)
            {
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User  not found."
                });
            }

            // Sign out the user
            await _signInManager.SignOutAsync();

            return Ok(new Response
            {
                Status = "Success",
                Message = "Logged out successfully."
            });
        }

        [HttpPut("profile")]
        [Authorize]
        [SwaggerOperation(Summary = "Edit user profile")]
        public async Task<IActionResult> EditProfile([FromBody] UserEditDto userEditDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.Email != userEditDto.Email)
            {
                var existingUser  = await _userManager.FindByEmailAsync(userEditDto.Email);
                if (existingUser  != null)
                {
                    return BadRequest(
                        new Response
                        {
                            Status = null,
                            Message = $"Email '{userEditDto.Email}' is already taken."
                        }
                    );
                }
            }
            
            user.Email = userEditDto.Email;
            user.UserName = userEditDto.Email;
            user.FullName = userEditDto.FullName;
            user.BirthDate = userEditDto.BirthDate;
            user.Gender = userEditDto.Gender;
            user.PhoneNumber = userEditDto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();

        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos;
using api.Dtos.User;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Service;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCredentials loginCredentials)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginCredentials.Email);
            if (user == null) {
                return Unauthorized("Login failed");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginCredentials.Password, false);
            if (!result.Succeeded) return Unauthorized("Login failed");

            return Ok(
                new TokenResponse
                {
                    Token = _tokenService.CreateToken(user)
                }
            );
        }

    }
}
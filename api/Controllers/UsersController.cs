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
        private readonly ApplicationDBContext _context;
        [ActivatorUtilitiesConstructorAttribute]
        public UsersController(ApplicationDBContext context, UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _context = context;
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
                if (registerDto.BirthDate > DateTime.Today)
                {
                    return BadRequest(new Response
                    {
                        Status = "Error",
                        Message = "Birth date can't be later than today"
                    });
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
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var isValidToken = await _tokenService.IsTokenValid(token);
            if (!isValidToken)
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
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            
            if (user == null)
            {
                return Unauthorized();
            }
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (token == null)
            {
                return Unauthorized();
            }
            var isValidToken = await _tokenService.IsTokenValid(token);
            if (!isValidToken)
            {
                return Unauthorized();
            }

            var userToken = new UserToken
            {
                Token = token,
                Username = username,
                ExpiryDate = DateTime.UtcNow.AddMinutes(60)
            };
            await CleanBlackList();
            _context.Tokens.Add(userToken);
            await _context.SaveChangesAsync();

            await _signInManager.SignOutAsync();

            return Ok(new Response
            {
                Status = "Success",
                Message = "Logged out successfully."
            });
        }

        private async Task CleanBlackList()
        {
            var tokens = await _context.Tokens.ToListAsync();
            foreach (var t in tokens)
            {
                if (t.ExpiryDate < DateTime.UtcNow)
                {
                    _context.Tokens.Remove(t);
                }
            }
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
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var isValidToken = await _tokenService.IsTokenValid(token);
            if (!isValidToken)
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
            
            if (user.FullName != userEditDto.FullName || user.BirthDate != userEditDto.BirthDate || user.Gender != userEditDto.Gender)
            {
                ChangeUserName(userEditDto, user);
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

        private void ChangeUserName(UserEditDto newUser, User user)
        {
            var author = _context.Authors.FirstOrDefault(a => a.UserId == user.Id);
            if (author != null)
            {
                author.FullName = newUser.FullName;
                author.BirthDate = newUser.BirthDate;
                author.Gender = newUser.Gender;
            }
            if (newUser.FullName != user.FullName) 
            {
                var posts = _context.Posts
                    .Where(p => p.AuthorId == new Guid(user.Id))
                    .ToList();
                foreach (var post in posts)
                {
                    post.Author = newUser.FullName;
                }
                var comments = _context.Comments
                    .Where(c => c.AuthorId == new Guid(user.Id))
                    .ToList();
                foreach (var comment in comments)
                {
                    comment.AuthorName = newUser.FullName;
                }   
            }
            
        }
    }
}
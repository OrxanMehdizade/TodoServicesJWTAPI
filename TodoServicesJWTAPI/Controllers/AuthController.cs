﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoServicesJWTAPI.Auth;
using TodoServicesJWTAPI.Configurations;
using TodoServicesJWTAPI.Models.DTOs.Auth;
using TodoServicesJWTAPI.Models.Entities;
using TodoServicesJWTAPI.Services.RabbitMQ;

namespace TodoServicesJWTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly RabbitMQConfiguration _rabbitMQConfiguration;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtService jwtService, IRabbitMQService rabbitMQService, RabbitMQConfiguration rabbitMQConfiguration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _rabbitMQService = rabbitMQService;
            _rabbitMQConfiguration = rabbitMQConfiguration;
        }

        private async Task<AuthTokenDto> GenerateToken(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var accessToken = _jwtService
                .GenerateSecurityToken(user.Id, user.Email!, roles, claims);
            var refreshToken = Guid.NewGuid().ToString().ToLower();
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);
            return new AuthTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }
        [HttpPost("Register")]
        public async Task<ActionResult<AuthTokenDto>> Register(RegisterRequest request)
        {
            _rabbitMQService.Publish<RegisterRequest>(request, _rabbitMQConfiguration.QueueName);
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
            {
                return Conflict("User already exist!");
            }
            
            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                RefreshToken = Guid.NewGuid().ToString().ToLower()
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return await GenerateToken(user);

        }
        [HttpPost("Login")]
        public async Task<ActionResult<AuthTokenDto>> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return BadRequest("User Not Found");
            }
            var canSignIn = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!canSignIn.Succeeded)
            {
                return BadRequest();
            }
            return await GenerateToken(user);
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult<AuthTokenDto>> RefreshToken(RefreshRequest request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
            if (user is null)
            {
                return Unauthorized();
            }
            return await GenerateToken(user);

        }

    }

}

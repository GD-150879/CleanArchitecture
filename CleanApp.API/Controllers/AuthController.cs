using CleanApp.Application.DTOs;
using CleanApp.Application.Interfaces;
using CleanApp.Application.Models;
using CleanApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CleanApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _authService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, [FromQuery] string role)
        {
            var result = await _authService.RegisterAsync(request,role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Error);
            }

            return Ok(new {Message=result.Success?.Message});            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null) return Unauthorized("Invalid credentials");
            return Ok(response);
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "User,Employee")]
        //[Authorize]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            try
            {
                var userClaims = User.Claims.Select(c => new { c.Type, c.Value });
                var users = await _authService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while retrieving users." + ex.Message.ToString());
            }
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto model)
        {
            if (string.IsNullOrWhiteSpace(model.RoleName))
                return BadRequest("Role name cannot be empty.");

            var result = await _authService.CreateRoleAsync(model.RoleName);

            if (result)
                return Ok("Role created successfully.");
            else
                return Conflict("Role already exists or invalid.");
        }
    }
}

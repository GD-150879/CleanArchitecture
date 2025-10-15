using CleanApp.Application.DTOs;
using CleanApp.Application.Interfaces;
using CleanApp.Application.Models;
using CleanApp.Domain.Entities;
using CleanApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;


        }

        public async Task<RegisterResult> RegisterAsync(RegisterRequest model, string roleName)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNo,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return new RegisterResult
                    {
                        Succeeded = false,
                        Error = new ErrorResponse
                        {
                            Message = "Registration failed",
                            Errors = errors
                        }
                    };
                }

                // ✅ Assign selected role (if exists)
                if (await _roleManager.RoleExistsAsync(roleName))
                    await _userManager.AddToRoleAsync(user, roleName);
                else
                    return new RegisterResult
                    {
                        Succeeded = false,
                        Error = new ErrorResponse { Message = $"Role '{roleName}' does not exist." }
                    };

                return new RegisterResult
                {
                    Succeeded = true,
                    Success = new SuccessResponse { Message = $"User created successfully with role '{roleName}'." }
                };
            }
            catch (Exception ex)
            {
                return new RegisterResult
                {
                    Succeeded = false,
                    Error = new ErrorResponse
                    {
                        Message = "An unexpected error occurred.",
                        Errors = new[] { ex.Message }
                    }
                };
            }
        }

        //public async Task<RegisterResult> RegisterAsync(RegisterRequest model, string roleName)
        //{
        //    try
        //    {
        //        var user = new ApplicationUser
        //        {
        //            Email = model.Email,
        //            UserName = model.Email,
        //            FullName = model.FullName,
        //            PhoneNumber = model.PhoneNo,
        //        };

        //        var result = await _userManager.CreateAsync(user, model.Password);

        //        if (result == null)
        //        {
        //            return new RegisterResult
        //            {
        //                Succeeded = false,
        //                Error = new ErrorResponse { Message = "User creation failed." }
        //            };
        //        }

        //        if (result.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(user, "User");
        //            return new RegisterResult
        //            {
        //                Succeeded = true,
        //                Success = new SuccessResponse { Message = "User Created Successfully" }
        //            };
        //        }
        //        else
        //        {
        //            var errors = result.Errors.Select(e => e.Description);
        //            return new RegisterResult
        //            {
        //                Succeeded = false,
        //                Error = new ErrorResponse
        //                {
        //                    Message = "Registration failed",
        //                    Errors = errors
        //                }
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new RegisterResult
        //        {
        //            Succeeded = false,
        //            Error = new ErrorResponse
        //            {
        //                Message = "An unexpected error occurred.",
        //                Errors = new[] { ex.Message }
        //            }
        //        };
        //    }
        //}

        public async Task<List<UserDto>> GetAllUsers()
        {
            try
            {

                var result = await _userManager.Users.ToListAsync();                
                var userDtos = new List<UserDto>();
                if (result != null)
                {
                    foreach (var user in result)
                    {
                        userDtos.Add(new UserDto
                        {
                            UserName = user.FullName,
                            Email = user.Email
                        });
                    }
                }
                return userDtos;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return null;
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return null;
            }

            //var token = GenerateJwtToken(user, out DateTime expiry);

            var (token, expiry) = await GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                Expires = expiry
            };


            //return new AuthResponse
            //{
            //    Token = token,
            //    Expires = expiry
            //};
        }

        private async Task<(string Token, DateTime Expiry)> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add role claims
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = 60;
            var expiryConfig = _config["JwtSettings:ExpiryMinutes"];
            if (!string.IsNullOrEmpty(expiryConfig) && int.TryParse(expiryConfig, out var minutes))
            {
                expiryMinutes = minutes;
            }

            var expiry = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenString, expiry);
        }

        private string GenerateJwtToken1(ApplicationUser user, out DateTime expiry)
        {
            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Email, user.Email),
             new Claim(ClaimTypes.NameIdentifier, user.Id),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
           };





            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = 60;
            var expiryConfig = _config["JwtSettings:ExpiryMinutes"];
            if (!string.IsNullOrEmpty(expiryConfig) && int.TryParse(expiryConfig, out var minutes))
            {
                expiryMinutes = minutes;
            }

            expiry = DateTime.Now.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            // Check if the role already exists
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
                return false;

            // Create the role
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            return result.Succeeded;
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await Task.FromResult(_roleManager.Roles.Select(r => r.Name).ToList());
        }
    }
}

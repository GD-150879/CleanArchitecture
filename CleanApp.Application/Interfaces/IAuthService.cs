using CleanApp.Application.DTOs;
using CleanApp.Application.Models;
using CleanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResult> RegisterAsync(RegisterRequest model, string roleName);
        Task<AuthResponse> LoginAsync(LoginRequest model);
        Task<bool> CreateRoleAsync(string roleName);
        Task<List<UserDto>> GetAllUsers();
        Task<List<string>> GetAllRolesAsync();

    }
}

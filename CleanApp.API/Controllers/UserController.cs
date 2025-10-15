using Azure.Core;
using CleanApp.Application.Interfaces;
using CleanApp.Domain.Entities;
using CleanApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService authService)
        {
            _userService = authService;
        }

        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser([FromBody] EmployeeDto employee)
        {
            if (ModelState.IsValid)
            {
                var addedEmployee = await _userService.AddEmployee(employee);
                if (addedEmployee != null)
                {
                    return Ok(addedEmployee);
                }
                else
                {
                    return BadRequest("User Cannot be Created");
                }
            }
            else
            {
                return BadRequest("Failed to Add User Data");
            }
        }

        

        [HttpGet("getAllUsers")]
        public async Task<List<EmployeeDto>> GetAllUSers()
        {
            var result = await _userService.GetAllEmployees();
            return result;
        }

        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _userService.GetEmployeesById(id);
            if (employee == null)
                return Ok(new EmployeeDto());

            return Ok(employee);
        }

        [HttpGet("getUserByLastName/{lastName}")]         
        public async Task<IActionResult> GetEmployeeByLastName(string lastName)
        {
            var employees = await _userService.GetEmployeesByLastName(lastName);

            if (employees == null || employees.Count == 0)
                return NotFound($"No employees found with LastName = {lastName}");

            return Ok(employees);
        }

        [HttpPut("updateEmployee/{id}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] EmployeeDto employee)
        {

            var existingEmployee = await _userService.GetEmployeesById(id);
            if (existingEmployee == null)
            {
                return BadRequest("Employee not Found!");
            }


            CopyNonNullProperties(existingEmployee, employee);

            await _userService.UpdateEmployee(existingEmployee);
            return Ok("Updated Successfully");
        }
        public static void CopyNonNullProperties<T>(T source, T destination)
        {
            var properties = typeof(T).GetProperties()
           .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source);
                if (value != null)
                {
                    prop.SetValue(destination, value);
                }
            }
        }

    }
}

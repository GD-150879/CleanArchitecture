using CleanApp.Application.Interfaces;
using CleanApp.Domain.Entities;
using CleanApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<EmployeeDto> AddEmployee(EmployeeDto employeeDto)
        {
            var employee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Gender = employeeDto.Gender,
                Address = employeeDto.Address,
                City = employeeDto.City
            };

            await _applicationDbContext.Employees.AddAsync(employee);
            await _applicationDbContext.SaveChangesAsync();

            employeeDto.Id = employee.Id;
            return employeeDto;
        }

        public async Task<List<EmployeeDto>> GetAllEmployees()
        {
            var employees = await _applicationDbContext.Employees.ToListAsync();

            return employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Gender = e.Gender,
                Address = e.Address,
                City = e.City
            }).ToList();
        }

        public async Task<EmployeeDto?> GetEmployeesById(int id)
        {
            var employee = await _applicationDbContext.Employees.FindAsync(id);



            if (employee == null)
                return null;

            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Gender = employee.Gender,
                Address = employee.Address,
                City = employee.City
            };
        }
        public async Task<List<EmployeeDto>?> GetEmployeesByLastName(string lastName)
        {
            var employee = await _applicationDbContext.Employees.Where(e => e.LastName == lastName).ToListAsync();



            if (employee == null)
                return null;



            return employee.Select(employee => new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Gender = employee.Gender,
                Address = employee.Address,
                City = employee.City
            }).ToList();
        }

        public async Task UpdateEmployee(EmployeeDto employeeDto)
        {
            var employee = await _applicationDbContext.Employees.FindAsync(employeeDto.Id);

            if (employee == null)
                throw new Exception($"Employee with ID {employeeDto.Id} not found.");

           

            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.Gender = employeeDto.Gender;
            employee.Address = employeeDto.Address;
            employee.City = employeeDto.City;



            _applicationDbContext.Employees.Update(employee);
            await _applicationDbContext.SaveChangesAsync();
        }

        
    }
}

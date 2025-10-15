using CleanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Application.Interfaces
{
    public interface IUserService
    {
        Task<EmployeeDto> AddEmployee(EmployeeDto employee);

        Task<List<EmployeeDto>> GetAllEmployees();
        Task<EmployeeDto> GetEmployeesById(int id);
        Task<List<EmployeeDto>> GetEmployeesByLastName(string lastName);
        Task UpdateEmployee(EmployeeDto employee);

    }
}

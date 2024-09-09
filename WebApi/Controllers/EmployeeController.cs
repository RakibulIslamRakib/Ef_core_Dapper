using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using WebApi.DTOs;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IApplicationReadDbConnection _readDbConnection;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public EmployeeController( IApplicationReadDbConnection readDbConnection, IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _readDbConnection = readDbConnection;
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var query = $"SELECT * FROM Employees";
            var employees = await _readDbConnection.QueryAsync<Employee>(query);
            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewEmployeeWithDepartment(EmployeeDto employeeDto)
        {
            if (employeeDto.Name == null || employeeDto.Department.Name == null)
                return BadRequest("Name cannot be null or empty");
            try
            {
                // Check if Department already exists (by Name)
                var department = await _departmentService.GetDepartmentExistsByNameAsync(employeeDto.Department.Name);

                if (department == null)
                {
                     department = new Department
                       {
                            Name = employeeDto.Department.Name,
                            Description = employeeDto.Department.Description
                       };

                    await _departmentService.AddDepartmentAsync(department);
                }

 

                // Check if the departmentId is valid (not zero)
                if (department.Id == 0)
                {
                    throw new Exception("Failed to insert Department");
                }

                // Add Employee using Employee Service
                var employee = new Employee
                {
                    DepartmentId = department.Id,
                    Name = employeeDto.Name,
                    Email = employeeDto.Email
                };

                await _employeeService.AddEmployeeAsync(employee);

                // Return the created employee's ID
                return Ok(employee.Id);
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}

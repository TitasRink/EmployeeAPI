using EmployeeAPI.Data;
using EmployeeAPI.Models;
using EmployeeAPI.Models.DTO;
using EmployeeAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Xml.Linq;

namespace EmployeeAPI.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeDbContext _dbContext;

        public EmployeeRepository(EmployeeDbContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<Employee> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);

            return employee;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByNameAndDateAsync(
            [FromQuery] string name,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var employees = await Task.Run(() => _dbContext.Employees.AsQueryable()
                .Where(e => (e.FirstName.Contains(name)) &&
                e.Birthdate >= startDate &&
                e.Birthdate <= endDate).ToList());

            return employees;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            var query = (from employees in _dbContext.Employees
                         select employees).ToListAsync();

            return await query;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByBossRole(string bossRole)
        {
            var employees = await Task.Run(() => _dbContext.Employees.AsQueryable()
            .Where(e => e.Boss == bossRole).ToList());

            return employees;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByRole(string role)
        {
            var employees = await Task.Run(() => _dbContext.Employees.AsQueryable()
             .Where(e =>e.Role == role).ToList());

            return employees;
        }

        public async Task<Employee> AddEmployeeAsync(EmployeeDTO employeeDTO)
        {
            Employee employee = new()
            {
                HomeAddress = employeeDTO.HomeAddress,
                EmploymentDate = employeeDTO.EmploymentDate,
                LastName = employeeDTO.LastName,
                FirstName = employeeDTO.FirstName,
                Role = employeeDTO.Role,
                Birthdate = employeeDTO.Birthdate,
                CurrentSalary = employeeDTO.CurrentSalary,
                Boss = employeeDTO.Boss
            };

            _dbContext.Employees.Add(employee);
            await _dbContext.SaveChangesAsync();

            return employee;
        }

        public async Task<EmployeeDTO> UpdateEmployeeAsync(Employee employeeOld, EmployeeDTO employeeNew)
        {
            _dbContext.Entry(employeeOld).CurrentValues.SetValues(employeeNew);
            await _dbContext.SaveChangesAsync();

            return employeeNew;
        }

        public async Task<Employee> UpdateEmployeeSalaryAsync(Employee employee, int salary)
        {
            employee.CurrentSalary = salary;
            await _dbContext.SaveChangesAsync();

            return employee;
        }

        public async Task<Employee> DeleteEmployeeAsync(Employee employee)
        {
            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();

            return employee;
        }
    }
}

using EmployeeAPI.Data;
using EmployeeAPI.Models;
using EmployeeAPI.Models.DTO;
using EmployeeAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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

            if (employee == null)
                return null;

            return employee;
        }

        public IEnumerable<Employee> GetEmployeesByNameAndDate(
            [FromQuery] string name,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var employees = _dbContext.Employees
                .Where(e => (e.FirstName.Contains(name)) &&
                e.Birthdate >= startDate &&
                e.Birthdate <= endDate).ToList();

            return employees;
        }

        public IEnumerable<Employee> GetEmployees()
        {
            var query = _dbContext.Employees.ToList();

            return query;
        }

        public IEnumerable<Employee> GetEmployeesByBossRole(string bossRole)
        {
            var employees = _dbContext.Employees.Where(e => e.Boss == bossRole).ToList();

            return employees;
        }

        public IEnumerable<Employee> GetEmployeesByRole(string role)
        {
            var employees = _dbContext.Employees.Where(e => e.Role == role).ToList();

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

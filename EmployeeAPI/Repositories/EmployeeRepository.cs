using EmployeeAPI.Data;
using EmployeeAPI.Models;
using EmployeeAPI.Models.DTO;
using EmployeeAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            return employee;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesBynameAndDateAsync(
            [FromQuery] string name,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var employees = await Task.Run(() => _dbContext.Employees.AsQueryable()
                .Where(e => (!string.IsNullOrEmpty(name) &&
                e.FirstName.Contains(name)) &&
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

        public async Task<IEnumerable<Employee>> GetEmployeesByBossId(Guid bossId)
        {
            Employee employee = await GetEmployeeByIdAsync(bossId);
            string bossName = employee.Boss;
            var employees = _dbContext.Employees.AsQueryable();
            var employeesList = employees.Where(e => e.Boss == bossName).ToList();
            return employeesList;
        }

        public async Task<IEnumerable<Employee>> GetEmployeeAvarageSalaryByRole()
        {
            var employees = await GetEmployeesAsync();

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

        public async Task<Employee> UpdateEmployeeAsync(Guid employeeId, EmployeeDTO employeeDTO)
        {
            Employee employeeQuery = await GetEmployeeByIdAsync(employeeId);
            _dbContext.Entry(employeeQuery).CurrentValues.SetValues(employeeDTO);
            await _dbContext.SaveChangesAsync();

            return employeeQuery;
        }

        public async Task<Employee> UpdateEmployeeSalaryAsync(Guid employeeId, int salary)
        {
            Employee employeeQuery = await GetEmployeeByIdAsync(employeeId);
            employeeQuery.CurrentSalary = salary;
            await _dbContext.SaveChangesAsync();

            return employeeQuery;
        }

        public async Task<Employee> DeleteEmployeeAsync(Guid employeeId)
        {
            Employee employee = await GetEmployeeByIdAsync(employeeId);
            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();

            return employee;
        }
    }
}

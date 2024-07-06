using EmployeeAPI.Models;
using EmployeeAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Repositories.Interface
{
    public interface IEmployeeRepository
    {
        Task<Employee> AddEmployeeAsync(EmployeeDTO employeeDTO);
        Task<Employee> DeleteEmployeeAsync(Employee employee);
        IEnumerable<Employee> GetEmployeesByRole(string role);
        Task<Employee> GetEmployeeByIdAsync(Guid id);
        IEnumerable<Employee> GetEmployees();
        IEnumerable<Employee> GetEmployeesByNameAndDate([FromQuery] string name, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate);
        IEnumerable<Employee> GetEmployeesByBossRole(string bossRole);
        Task<EmployeeDTO> UpdateEmployeeAsync(Employee employeeOld, EmployeeDTO employeeNew);
        Task<Employee> UpdateEmployeeSalaryAsync(Employee employee, int salary);
    }
}
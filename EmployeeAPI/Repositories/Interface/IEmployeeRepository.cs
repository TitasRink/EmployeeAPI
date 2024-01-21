using EmployeeAPI.Models;
using EmployeeAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Repositories.Interface
{
    public interface IEmployeeRepository
    {
        Task<Employee> AddEmployeeAsync(EmployeeDTO employeeDTO);
        Task<Employee> DeleteEmployeeAsync(Employee employee);
        Task<IEnumerable<Employee>> GetEmployeesByRole(string role);
        Task<Employee> GetEmployeeByIdAsync(Guid id);
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<IEnumerable<Employee>> GetEmployeesByNameAndDateAsync([FromQuery] string name, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate);
        Task<IEnumerable<Employee>> GetEmployeesByBossRole(string bossRole);
        Task<EmployeeDTO> UpdateEmployeeAsync(Employee employeeOld, EmployeeDTO employeeNew);
        Task<Employee> UpdateEmployeeSalaryAsync(Employee employee, int salary);
    }
}
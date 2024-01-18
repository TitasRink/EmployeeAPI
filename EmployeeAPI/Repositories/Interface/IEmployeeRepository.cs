using EmployeeAPI.Models;
using EmployeeAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Repositories.Interface
{
    public interface IEmployeeRepository
    {
        Task<Employee> AddEmployeeAsync(EmployeeDTO employeeDTO);
        Task<Employee> DeleteEmployeeAsync(Guid employeeId);
        Task<IEnumerable<Employee>> GetEmployeeAvarageSalaryByRole();
        Task<Employee> GetEmployeeByIdAsync(Guid id);
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<IEnumerable<Employee>> GetEmployeesBynameAndDateAsync([FromQuery] string name, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate);
        Task<IEnumerable<Employee>> GetEmployeesByBossId(Guid bossId);
        Task<Employee> UpdateEmployeeAsync(Guid employeeId, EmployeeDTO employeeDTO);
        Task<Employee> UpdateEmployeeSalaryAsync(Guid employeeId, int salary);
    }
}
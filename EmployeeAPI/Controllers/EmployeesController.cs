using EmployeeAPI.Middlewares;
using EmployeeAPI.Models;
using EmployeeAPI.Models.DTO;
using EmployeeAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet("GetEmployees")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await _employeeRepository.GetEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                ExeptionHandlerMiddleware.LogException(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("employees/{id}")]
        public async Task<IActionResult> GetEmployee([FromRoute] Guid id)
        {
            try
            {
                Employee employee = await _employeeRepository.GetEmployeeByIdAsync(id);
                return employee == null ? throw new Exception("No Employee found") : (IActionResult)Ok(employee);
            }
            catch (Exception ex)
            {
                ExeptionHandlerMiddleware.LogException(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetEmployeeAvarageSalaryByRole")]
        public async Task<IActionResult> GetEmployeeAvarageSalaryByRole(string role)
        {
            try
            {
                if (string.IsNullOrEmpty(role))
                {
                    throw new Exception("No Role found");
                }
                var employees = await _employeeRepository.GetEmployeeAvarageSalaryByRole() ?? 
                    throw new Exception($"No Employees found with role: {role}");
                var roleEmployees = employees.Where(e => e.Role == role);
                int employeeCount = roleEmployees.Count();
                decimal averageSalary = roleEmployees.Average(e => e.CurrentSalary);

                return Ok($"Employee Count = {employeeCount}, Average Salary = {averageSalary}");
            }
            catch (Exception ex)
            {
                ExeptionHandlerMiddleware.LogException(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetEmployeesByBossId/{id}")]
        public async Task<IActionResult> GetEmployeesByBossId(Guid id)
        {
            try
            {
                if (string.IsNullOrEmpty(id.ToString()))
                { 
                    throw new Exception($"No Employee found with id: {id}");
                }

                var employees = await _employeeRepository.GetEmployeesByBossId(id);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                ExeptionHandlerMiddleware.LogException(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetEmployeesBynameAndDateAsync")]
        public async Task<IActionResult> GetEmployeesBynameAndDateAsync(string name, DateTime startDate, DateTime endDate)
        {
            try
            {
                var employees = await _employeeRepository.GetEmployeesBynameAndDateAsync(name, startDate, endDate);

                return Ok(employees);
            }
            catch (Exception ex)
            {
                ExeptionHandlerMiddleware.LogException(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> PostEmployee(EmployeeDTO employeeDTO)
        {
            try
            {
                if (employeeDTO == null)
                {
                    throw new Exception("No Employee found");
                }

                if (CheckIfCeoExist() && employeeDTO.Role == "CEO")
                {
                    throw new Exception("There can be only one employee with CEO role.");
                }

                Employee employee = await _employeeRepository.AddEmployeeAsync(employeeDTO);
                if (employee == null)
                {
                    return NotFound();
                }
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)
        {
            try
            {
                Employee employee = await _employeeRepository.DeleteEmployeeAsync(id);

                return employee == null ? throw new Exception("No Employee found") : (IActionResult)Ok(employee);
            }
            catch (Exception ex)
            {
                ExeptionHandlerMiddleware.LogException(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateEmployee/{id}")]
        public async Task<IActionResult> PutEmployee([FromRoute] Guid id, EmployeeDTO employeeDTO)
        {
            try
            {
                if (employeeDTO == null)
                {
                    throw new Exception("No Employee found");
                }

                if (CheckIfCeoExist() && employeeDTO.Role == "CEO")
                {
                    throw new Exception("There can be only one employee with CEO role.");
                }

                Employee updatedEmployee = await _employeeRepository.UpdateEmployeeAsync(id, employeeDTO);
              
                return Ok(updatedEmployee);
            }
            catch (Exception ex)
            {
                ExeptionHandlerMiddleware.LogException(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateEmployeeSalaryAsync/{id}")]
        public async Task<IActionResult> UpdateEmployeeSalaryAsync(Guid id, int salary)
        {
            try
            {
                if (await _employeeRepository.GetEmployeeByIdAsync(id) == null)
                {
                    return BadRequest($"No employee found with id : {id}");
                }

                if (salary < 1) 
                {
                    return BadRequest("Salary must be greater then 0");
                }

                var updatedEmployee = await _employeeRepository.UpdateEmployeeSalaryAsync(id, salary);
                if (updatedEmployee == null)
                {
                    return NotFound();
                }

                return Ok(updatedEmployee);
            }
            catch (Exception ex)
            {
                ExeptionHandlerMiddleware.LogException(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        private bool CheckIfCeoExist()
        {
            var employees = _employeeRepository.GetEmployeesAsync();
            var ceoCount = employees.Result.FirstOrDefault(e => e.Role == "CEO");

            if (ceoCount == null)
            {
                return false;
            }

            return true;
        }
    }
}

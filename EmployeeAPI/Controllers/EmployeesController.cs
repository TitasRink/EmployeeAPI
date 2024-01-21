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
                if (employees == null)
                {
                    return NotFound("No Employee found");
                }

                return Ok(employees);
            }
            catch (Exception ex)
            {
                ExeptionHandlerMiddleware.LogException(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("employees/{id}")]
        public async Task<IActionResult> GetEmployee(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest($"Field can not be empty {id}");
                }

                Employee employee = await _employeeRepository.GetEmployeeByIdAsync(id);

                if (employee == null)
                {
                    return NotFound("No Employee found");
                }
                
                return Ok(employee);
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
                    return BadRequest($"Field can not be empty {role}");
                }

                var employees = await _employeeRepository.GetEmployeesByRole(role);
                if (employees == null)
                {
                    return NotFound($"No Employees found with role: {role}");
                }

                int employeeCount = employees.Count();
                decimal averageSalary = employees.Average(e => e.CurrentSalary);

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
                if (id == Guid.Empty)
                {
                    return BadRequest($"Field can not be empty {id}");
                }

                var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
                var employees = await _employeeRepository.GetEmployeesByBossRole(employee.Boss);
                if (employees == null)
                { 
                    return NotFound();
                }

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
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(startDate.ToString()) || string.IsNullOrEmpty(endDate.ToString()))
                {
                    return BadRequest("Fields can not be empty");
                }

                var employees = await _employeeRepository.GetEmployeesByNameAndDateAsync(name, startDate, endDate);
                if (employees == null) 
                {
                    return BadRequest();
                }

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
                    return BadRequest("Fill employee field");
                }

                if (CheckIfCeoExist() && employeeDTO.Role == "CEO")
                {
                    return BadRequest("There can be only one employee with CEO role.");
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
                ExeptionHandlerMiddleware.LogException(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Fill Id field");
                }

                Employee employee = _employeeRepository.GetEmployeeByIdAsync(id).Result;
                if (employee == null)
                {
                    return NotFound("No Employee found");
                }

                employee = await _employeeRepository.DeleteEmployeeAsync(employee);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                ExeptionHandlerMiddleware.LogException(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateEmployee/{id}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] Guid id, EmployeeDTO employeeDTO)
        {
            try
            {
                if (employeeDTO == null || id == Guid.Empty)
                {
                    return BadRequest("No Employee found");
                }

                if (CheckIfCeoExist() && employeeDTO.Role == "CEO")
                {
                    return BadRequest("There can be only one employee with CEO role.");
                }

                Employee employee = _employeeRepository.GetEmployeeByIdAsync(id).Result;
                EmployeeDTO updatedEmployee = await _employeeRepository.UpdateEmployeeAsync(employee, employeeDTO);
                if (updatedEmployee == null) 
                {
                    return BadRequest($"Employee: {employee} not updated"); 
                }

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
                if (_employeeRepository.GetEmployeeByIdAsync(id) == null)
                {
                    return NotFound($"No employee found with id : {id}");
                }

                if (salary < 1) 
                {
                    return BadRequest("Salary must be greater then 0");
                }
                Employee employee = await _employeeRepository.GetEmployeeByIdAsync(id);
                Employee updatedEmployee = await _employeeRepository.UpdateEmployeeSalaryAsync(employee, salary);
                if (updatedEmployee == null)
                {
                    return BadRequest($"Employee: {employee} salary not updated");
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

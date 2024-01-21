using EmployeeAPI.Controllers;
using EmployeeAPI.Models;
using EmployeeAPI.Models.DTO;
using EmployeeAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EmployeeApiTest
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepo;
        private readonly EmployeesController _controller;
        private readonly Employee employeeOne;
        private readonly Employee employeeTwo;
        private readonly Employee employeeThree;

        public EmployeesControllerTests()
        {
            _mockRepo = new Mock<IEmployeeRepository>();
            _controller = new EmployeesController(_mockRepo.Object);
            employeeOne = new()
            {
                FirstName = "John",
                LastName = "Doe",
                Birthdate = new DateTime(2002, 6, 12),
                EmploymentDate = new DateTime(2020, 8, 22),
                HomeAddress = "New",
                CurrentSalary = 50000,
                Role = "CEO",
                Boss = "John"
            };
            employeeTwo = new()
            {
                FirstName = "John",
                LastName = "Doe",
                Birthdate = new DateTime(2002, 6, 12),
                EmploymentDate = new DateTime(2020, 8, 22),
                HomeAddress = "New",
                CurrentSalary = 30000,
                Role = "Dev",
                Boss = "John"
            };
            employeeThree = new()
            {
                FirstName = "Adam",
                LastName = "Doe",
                Birthdate = new DateTime(1986, 6, 12),
                EmploymentDate = new DateTime(2016, 8, 22),
                HomeAddress = "New",
                CurrentSalary = 20000,
                Role = "Dev",
                Boss = "Tom"
            };
        }

        [Fact]
        public async Task GetEmployees_WhenCalled_ReturnsEmployeesFromRepo()
        {
            // Arrange

            // Act
            _mockRepo.Setup(repo => repo.GetEmployeesAsync()).ReturnsAsync(new List<Employee> { employeeOne, employeeTwo });

            var result = await _controller.GetEmployees();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var employees = Assert.IsAssignableFrom<IEnumerable<Employee>>(okResult.Value);

            // Assert
            Assert.Equal(2, employees.Count());
            Assert.Equal(employeeOne, employees.First());
        }

        [Fact]
        public async Task GetEmployee_WhenCalled_ReturnEmployee()
        {
            // Arrange
            Employee employee = await GetFirstEmploye();

            // Act
            _mockRepo.Setup(repo => repo.GetEmployeeByIdAsync(employee.Id)).ReturnsAsync(employeeOne);
            var result = await _controller.GetEmployee(employee.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var employeeValue = Assert.IsType<Employee>(okResult.Value);

            // Assert
            Assert.Equal(employee.Id, employee.Id);
            Assert.Equal(employeeOne.FirstName, employeeValue.FirstName);
        }

        [Fact]
        public async Task GetEmployeeAvarageSalaryByRole_WhenCalled_AvarageSalary()
        {
            // Arrange
            string role = "Dev";
            string avarageSalary = "25000";

            // Act
            _mockRepo.Setup(repo => repo.GetEmployeesByRole(role)).ReturnsAsync(new List<Employee> { employeeTwo, employeeThree });

            var result = await _controller.GetEmployeeAvarageSalaryByRole(role);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultAvarageSalary = Assert.IsType<string>(okResult.Value);

            // Assert
            Assert.Contains("Employee Count = ", resultAvarageSalary);
            Assert.Contains("Average Salary = ", resultAvarageSalary);
            Assert.Contains(avarageSalary, resultAvarageSalary);
        }

        [Fact]
        public async Task GetEmployeesByBossId_WhenCalled_ReturnsEmployeesByBossRole()
        {
            // Arrange
            string role = "John";

            Employee employeeFirst = await GetFirstEmploye();

            // Act
            _mockRepo.Setup(repo => repo.GetEmployeeByIdAsync(employeeFirst.Id)).ReturnsAsync(employeeOne);
            _mockRepo.Setup(repo => repo.GetEmployeesByBossRole(role)).ReturnsAsync(new List<Employee> { employeeOne, employeeTwo });

            var result = await _controller.GetEmployeesByBossId(employeeFirst.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultEmployees = Assert.IsAssignableFrom<IEnumerable<Employee>>(okResult.Value);

            // Assert
            Assert.True(resultEmployees.Any());
            foreach (var employee in resultEmployees)
            {
                Assert.Equal(role, employee.Boss);
            }
        }

        [Fact]
        public async Task PostEmployee_WhenCalled_AddsEmployee()
        {
            // Arrange
            EmployeeDTO employeeDTO = CreateEmployeeDTO();

            // Act
            _mockRepo.Setup(repo => repo.AddEmployeeAsync(employeeDTO)).ReturnsAsync(employeeOne);

            var result = await _controller.PostEmployee(employeeDTO);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var employee = Assert.IsType<Employee>(okResult.Value);

            // Assert
            Assert.Equal(employeeDTO.FirstName, employee.FirstName);
        }

        [Fact]
        public async Task DeleteEmployee_WhenCalled_DeleteEmployee()
        {
            // Arrange

            // Act
            Employee employee = await GetFirstEmploye();

            _mockRepo.Setup(repo => repo.GetEmployeeByIdAsync(employee.Id)).ReturnsAsync(employeeOne);
            _mockRepo.Setup(repo => repo.DeleteEmployeeAsync(employee)).ReturnsAsync(employeeOne);

            var result = await _controller.DeleteEmployee(employee.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateEmployee_WhenCalled_ReturnsUpdatedEmployee()
        {
            // Arrange
            EmployeeDTO employeeDTO = CreateEmployeeDTO();
            employeeDTO.LastName = "Updated";
            employeeDTO.Role = "Test";

            // Act
            Employee employee = await GetFirstEmploye();

            _mockRepo.Setup(repo => repo.GetEmployeeByIdAsync(employee.Id)).ReturnsAsync(employeeOne);
            _mockRepo.Setup(repo => repo.UpdateEmployeeAsync(employeeOne, employeeDTO)).ReturnsAsync(employeeDTO);

            var employeeUpdated = await _controller.UpdateEmployee(employee.Id, employeeDTO);
            var employeeOkResultUpdated = Assert.IsType<OkObjectResult>(employeeUpdated);
            var employeeUpdatedValue = Assert.IsType<EmployeeDTO>(employeeOkResultUpdated.Value);

            // Assert
            Assert.True(employeeUpdatedValue.LastName == employeeDTO.LastName);
        }

        [Fact]
        public async Task UpdateEmployeeSalaryAsync_WhenCalled_ReturnsUpdatedEmployee()
        {
            // Arrange
            int salaryNew = 3000;

            // Act
            Employee employee = await GetFirstEmploye();

            _mockRepo.Setup(repo => repo.GetEmployeeByIdAsync(employee.Id)).ReturnsAsync(employeeOne);
            employeeOne.CurrentSalary = salaryNew;
            _mockRepo.Setup(repo => repo.UpdateEmployeeSalaryAsync(employeeOne, salaryNew)).ReturnsAsync(employeeOne);

            var employeeUpdated = await _controller.UpdateEmployeeSalaryAsync(employee.Id, salaryNew);
            var employeeOkResultUpdated = Assert.IsType<OkObjectResult>(employeeUpdated);
            var employeeUpdatedValue = Assert.IsType<Employee>(employeeOkResultUpdated.Value);

            // Assert
            Assert.True(employeeUpdatedValue.CurrentSalary == salaryNew);
        }

        private async Task<Employee> GetFirstEmploye()
        {
            _mockRepo.Setup(repo => repo.GetEmployeesAsync()).ReturnsAsync(new List<Employee> { employeeOne, employeeTwo, employeeThree });
            
            var employees = await _controller.GetEmployees();
            var employeeOkResult = Assert.IsType<OkObjectResult>(employees);
            var employeeFirst = Assert.IsAssignableFrom<IEnumerable<Employee>>(employeeOkResult.Value).First();

            return employeeFirst;
        }

        private static EmployeeDTO CreateEmployeeDTO()
        {
            return new()
            {
                FirstName = "John",
                LastName = "Doe",
                Birthdate = new DateTime(2002, 6, 12),
                EmploymentDate = new DateTime(2020, 8, 22),
                HomeAddress = "New",
                CurrentSalary = 50000,
                Role = "CEO",
                Boss = "John"
            };
        }
    }
}
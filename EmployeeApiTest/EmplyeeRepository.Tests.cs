using EmployeeAPI.Controllers;
using EmployeeAPI.Data;
using EmployeeAPI.Models.DTO;
using EmployeeAPI.Repositories;
using EmployeeAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EmployeeApiTest
{
    public class EmplyeeRepository : DbContext
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly EmployeeDTO employeeOne;
        private readonly EmployeeDTO employeeTwo;
        private readonly EmployeeDTO employeeThree;

        private readonly Mock<IEmployeeRepository> _mockRepo;
        private readonly EmployeesController _controller;

        public EmplyeeRepository()
        {
            _mockRepo = new Mock<IEmployeeRepository>();
            _controller = new EmployeesController(_mockRepo.Object);
            employeeRepository = new EmployeeRepository(SetContextBuilderOptions());
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
        public async Task AddEmployeeAsync_WhenCalled_ShouldAddEmployeeToRepository()
        {
            // Arrange
            await employeeRepository.AddEmployeeAsync(employeeOne);

            // Act
            var employee = employeeRepository.GetEmployeesAsync().Result.FirstOrDefault();

            // Assert
            Assert.Equal(employee.FirstName, employeeOne.FirstName);
        }

        [Fact]
        public async Task GetEmployeesAsync_WhenCalled_TwoEmployeesReturned()
        {
            // Arrange
            await employeeRepository.AddEmployeeAsync(employeeOne);
            await employeeRepository.AddEmployeeAsync(employeeTwo);

            // Act
            var employee = employeeRepository.GetEmployeesAsync().Result;

            // Assert
            Assert.Equal(2, employee.Count());
        }

        [Fact]
        public async Task AddEmployeeAsync_WhenCalled_ThrowsExeption()
        {
            // Arrange
            employeeOne.FirstName = "Test";
            employeeOne.LastName = "Test";
            string errorMessage = "First name cannot be the same as last name.";

            // Act
            var ex = await Assert.ThrowsAsync<Exception>(
                async () => await employeeRepository.AddEmployeeAsync(employeeOne));

            // Assert
            Assert.Contains(errorMessage, ex.Message);
        }

        [Fact]
        public async Task GetEmployeesByRole_WhenCalled_ShouldReturnEmployeesWithSameRole()
        {
            // Arrange
            string role = "Dev";
            await employeeRepository.AddEmployeeAsync(employeeThree);
            await employeeRepository.AddEmployeeAsync(employeeTwo);

            // Act
            var employeesByRole = employeeRepository.GetEmployeesByRole(role).Result;

            // Assert
            Assert.Equal(2, employeesByRole.Count());
            foreach (var employee in employeesByRole)
            {
                Assert.Equal(role, employee.Role);
            }
        }

        [Fact]
        public async Task DeleteEmployeeAsync_WhenCalled_ShouldAddEmployeeToRepository()
        {
            // Arrange
            await employeeRepository.AddEmployeeAsync(employeeOne);

            // Act
            var employee = employeeRepository.GetEmployeesAsync().Result.FirstOrDefault();
            var employeeDeleted = employeeRepository.DeleteEmployeeAsync(employee).Result;

            // Assert
            Assert.Equal(employeeDeleted.FirstName, employee.FirstName);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WhenCalled_ReturnEmployee()
        {
            // Arrange
            await employeeRepository.AddEmployeeAsync(employeeOne);

            // Act
            Guid id = employeeRepository.GetEmployeesAsync().Result.FirstOrDefault().Id;
            var employee = employeeRepository.GetEmployeeByIdAsync(id).Result;

            // Assert
            Assert.Equal(id, employee.Id);
        }

        [Fact]
        public async Task GetEmployeesByNameAndDateAsync_WhenCalled_ReturnTwoEmployees()
        {
            // Arrange
            string name = "John";
            DateTime employmentEndDate = new(2023, 10, 12);
            DateTime employmentStartDate = new(2000, 10, 12);
            await employeeRepository.AddEmployeeAsync(employeeOne);
            await employeeRepository.AddEmployeeAsync(employeeTwo);
            await employeeRepository.AddEmployeeAsync(employeeThree);

            // Act
            var employees = employeeRepository.GetEmployeesByNameAndDateAsync(name, employmentStartDate, employmentEndDate).Result;

            // Assert
            Assert.Equal(2, employees.Count());
            foreach (var employee in employees)
            {
                Assert.Equal(name, employee.FirstName);
                Assert.True(employmentEndDate > employee.EmploymentDate);
            }
        }

        [Fact]
        public async Task GetEmployeesByBossRole_WhenCalled_ReturnsTwoEmployeesOrderByRole()
        {
            // Arrange
            string role = "Tom";
            await employeeRepository.AddEmployeeAsync(employeeOne);
            await employeeRepository.AddEmployeeAsync(employeeTwo);
            await employeeRepository.AddEmployeeAsync(employeeThree);

            // Act
            var employees = employeeRepository.GetEmployeesByBossRole(role).Result;

            // Assert
            foreach (var employee in employees)
            {
                Assert.Equal(role, employee.Boss);
            }
        }

        [Fact]
        public async Task UpdateEmployeeAsync_WhenCalled_ShouldReturnUpdatedEmployee()
        {
            // Arrange
            await employeeRepository.AddEmployeeAsync(employeeOne);
            await employeeRepository.AddEmployeeAsync(employeeTwo);

            // Act
            var employeeToUpdate = employeeRepository.GetEmployeesAsync().Result.FirstOrDefault();
            var employeeUpdated = employeeRepository.UpdateEmployeeAsync(employeeToUpdate, employeeTwo).Result;

            // Assert
            Assert.True(employeeUpdated != null);
            Assert.Equal(employeeUpdated, employeeTwo);
        }

        [Fact]
        public async Task UpdateEmployeeSalaryAsync_WhenCalled_ReturnEmployeeWithUpdatedSalary()
        {
            // Arrange
            int salaryNew = 200;
            decimal salaryOld = employeeOne.CurrentSalary;
            await employeeRepository.AddEmployeeAsync(employeeOne);

            // Act
            var employee = employeeRepository.GetEmployeesAsync().Result.FirstOrDefault();

            var employeeUpdated = employeeRepository.UpdateEmployeeSalaryAsync(employee, salaryNew).Result;

            // Assert
            Assert.False(employeeUpdated.CurrentSalary == salaryOld);
            Assert.Equal(employeeUpdated.CurrentSalary, salaryNew);
        }

        private static EmployeeDbContext SetContextBuilderOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EmployeeDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var context = new EmployeeDbContext(optionsBuilder.Options);
            return context;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models.DTO
{
    public class EmployeeDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime Birthdate { get; set; }

        [Required]
        public DateTime EmploymentDate { get; set; }

        [Required]
        public string HomeAddress { get; set; }

        [Required]
        public decimal CurrentSalary { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Boss { get; set; }
    }
}

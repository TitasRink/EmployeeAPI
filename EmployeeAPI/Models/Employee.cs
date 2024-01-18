using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models
{
    public class Employee : Ceo
    {
        private string _firstName;
        private string _lastName;
        private DateTime _birthdate;
        private DateTime _employmentDate;
        private decimal _currentSalary;

        public Employee()
        {
            Id = Guid.NewGuid();
        }

        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Code has to be a maximum of 50 characters")]
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (value != _lastName)
                {
                    _firstName = value;
                }
                else
                {
                    throw new Exception("First name cannot be the same as last name.");
                }
            }
        }

        [Required]
        [MaxLength(50, ErrorMessage = "Code has to be a maximum of 50 characters")]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (value != _firstName)
                {
                    _lastName = value;
                }
                else
                {
                    throw new Exception("Last name cannot be the same as first name.");
                }
            }
        }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Birthdate
        {
            get { return _birthdate; }
            set
            {
                var age = DateTime.Today.Year - value.Year;
                if (age < 18 || age > 70)
                {
                    throw new Exception("Employee must be at least 18 years old and not older than 70 years.");
                }
                _birthdate = value;
            }
        }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EmploymentDate
        {
            get { return _employmentDate; }
            set
            {
                if (value <= DateTime.Today)
                {
                    if (value >= new DateTime(2000, 1, 1))
                    {
                        _employmentDate = value;
                    }
                    else
                    {
                        throw new Exception("Employment date cannot be earlier than 2000-01-01.");
                    }
                }
                else
                {
                    throw new Exception("Employment date cannot be a future date.");
                }
            }
        }

        [Required]
        public string HomeAddress { get; set; }

        [Required]
        public decimal CurrentSalary
        {
            get { return _currentSalary; }
            set
            {
                if (value > 1)
                {
                    _currentSalary = value;
                }
                else
                {
                    throw new Exception("Salary must be greater then 0");
                }
            }
        }

        [Required]
        public string Role { get; set; }
        
    }
}

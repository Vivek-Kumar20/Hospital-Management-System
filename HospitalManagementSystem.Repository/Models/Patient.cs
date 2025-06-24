using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Repository.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required] // Ensures this column is always populated
        [DataType(DataType.DateTime)] // Specifies that RegistrationDate is a date and time
        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; }

        [Required(ErrorMessage = "Contact Number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact Number must be exactly 10 digits.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Contact Number must be 10 digits long.")]
        public string ContactNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        public string MedicalHistory { get; set; }
    }
}

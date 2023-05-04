using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MileStone_Attendance_Management.Models
{
    [Index(nameof(EmployeeId), IsUnique = true)]
    public class Employees
    {
        [Display(Name = "Employee ID")]
        [Required(ErrorMessage = "Employee ID is Required")]
        public string EmployeeId { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        [Key]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
        [ForeignKey("Roles")]
        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Designation is Required")]
        public string Designation { get; set; }

        [ForeignKey("Degrees")]
        [Display(Name = "Degree")]
        [Required(ErrorMessage = "Degree is Required")]
        public string NormalizedDegree { get; set; }

        [Display(Name = "Branch")]
        [Required(ErrorMessage = "Branch is Required")]
        public string NormalizedBranch { get; set; }
        [Display(Name = "Degree")]
        public virtual Degrees Degrees { get; set; }
        public virtual IdentityRole Roles { get; set; }
    }
}

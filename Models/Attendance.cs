using System.Numerics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MileStone_Attendance_Management.Models
{
    [Keyless]
    [Index(nameof(RollNumber), IsUnique = true)]
    public class Attendance
    {
        [Display(Name = "RollNumber")]
        [Required(ErrorMessage = "RollNumber is Required")]
        public int RollNumber { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        [Display(Name = "Present Status")]
        [Required(ErrorMessage = "Present Status is Required")]
        public Boolean PresentStatus { get; set; } = false;
        [ForeignKey("Degrees")]
        [Display(Name = "Degree")]
        [Required(ErrorMessage = "Degree is Required")]
        public string NormalizedDegree { get; set; }
        [Display(Name = "Normalized Branch")]
        [Required(ErrorMessage = "Normalized Branch Name is Required")]
        public string NormalizedBranch { get; set; }
        [ForeignKey("Courses")]
        [Display(Name = "Course Id")]
        [Required(ErrorMessage = "Course Id is Required")]
        public string CourseId { get; set; }
        [Display(Name = "Section")]
        [Required(ErrorMessage = "Section is Required")]
        public string Section { get; set; }
        [ForeignKey("Students")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
        public DateTime TimeStamp { get; set; }=DateTime.Now.ToLocalTime();
        public virtual Degrees Degrees { get; set; }
        public virtual Courses Courses { get; set; }
        public virtual Students Students { get; set; }


    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MileStone_Attendance_Management.Models
{
    public class CoursesAssigned
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Courses")]
        [Display(Name = "Course Id")]
        [Required(ErrorMessage = "Course Id is Required")]
        public string CourseId { get; set; }
        [Display(Name = "Semester")]
        [Required(ErrorMessage = "Semester is Required")]
        public int Semester { get; set; }

        [ForeignKey("Degrees")]
        [Display(Name = "Degree")]
        [Required(ErrorMessage = "Degree is Required")]
        public string NormalizedDegree { get; set; }

        [Display(Name = "Branch")]
        [Required(ErrorMessage = "Branch is Required")]
        public string NormalizedBranch { get; set; }
        
        public virtual Degrees? Degrees { get; set; }
        public virtual Courses? Courses { get; set; }
    }
}

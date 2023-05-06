using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
namespace MileStone_Attendance_Management.Models
{
    public class AttendanceHistory
    {
        [Key]
        public int AttendanceId { get; set; }
        [ForeignKey("Degrees")]
        [Display(Name = "Degree")]
        [Required(ErrorMessage = "Degree is Required")]
        public string? NormalizedDegree { get; set; }

        [Display(Name = "Branch")]
        [Required(ErrorMessage = "Branch is Required")]
        public string? NormalizedBranch { get; set; }
        [ForeignKey("Courses")]
        [Display(Name = "Course Id")]
        [Required(ErrorMessage = "Course Id is Required")]
        public string? CourseId { get; set; }
        [Display(Name = "Section")]
        [Required(ErrorMessage = "Section is Required")]
        public string? Section { get; set; }
        [Display(Name ="Batch")]
        [Required(ErrorMessage = "Batch is Required")]
        public string? Batch { get; set; }
        [Display(Name = "Date and Time")]
        public DateTime TimeStamp { get; set; }

        public virtual Degrees? Degrees { get; set; }
        public virtual Courses? Courses { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MileStone_Attendance_Management.Models
{
    public class Courses
    {
        [Key]
        [Display(Name = "Course Id")]
        [Required(ErrorMessage = "Course Id is Required")]
        public string CourseId { get; set; }
        [Display(Name = "Course Type")]
        [Required(ErrorMessage = "Course Type is Required")]
        public string CourseType { get; set; }
        [Display(Name = "Course Name")]
        [Required(ErrorMessage = "Course Name is Required")]
        public string CourseName { get; set;}
    }
}

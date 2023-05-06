using System.Numerics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MileStone_Attendance_Management.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("AttendanceHistory")]
        [Display(Name = "AttendanceId")]
        public int AttendanceId { get; set; }
        [Display(Name = "RollNumber")]
        [Required(ErrorMessage = "RollNumber is Required")]
        public int RollNumber { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is Required")]
        public string? Name { get; set; }
        [Display(Name = "Present Status")]
        [Required(ErrorMessage = "Present Status is Required")]
        public Boolean PresentStatus { get; set; }
        [ForeignKey("Students")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is Required")]
        public string? Email { get; set; }
        public virtual Students? Students { get; set; }
        public virtual AttendanceHistory? AttendanceHistory { get; set; }


    }
}

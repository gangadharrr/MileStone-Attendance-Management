using System.Numerics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MileStone_Attendance_Management.Models
{
    [Index(nameof(RollNumber), IsUnique = true)]
    public class Students
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        [Key]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
        [Display(Name = "Batch")]
        [Required(ErrorMessage = "Batch is Required")]
        public string Batch { get; set; }
        [Display(Name = "Section")]
        [Required(ErrorMessage = "Section is Required")]
        public string Section { get; set; }
        [Display(Name = "RollNumber")]
        [Required(ErrorMessage = "RollNumber is Required")]
        public int RollNumber { get; set;}
   
        [ForeignKey("Degrees")]
        [Display(Name = "Normalized Degree")]
        [Required(ErrorMessage = "Normalized Degree is Required")]
        public string NormalizedDegree { get; set; }
 
        [Display(Name = "Branch")]
        [Required(ErrorMessage = "Branch is Required")]
        public string NormalizedBranch { get; set; }
        [Display(Name = "Degree")]
        public virtual Degrees Degrees { get; set; }
    }
}

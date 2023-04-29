using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace MileStone_Attendance_Management.Models
{
    public class Branches
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Branch Name")]
        [Required(ErrorMessage = "Branch Name is Required")]
        public string Branch { get; set; }
        [Display(Name = "Normalized Branch")]
        [Required(ErrorMessage = "Normalized Branch Name is Required")]
        public string NormalizedBranch { get; set; }

        [Display(Name = "Degree Name")]
        [Required(ErrorMessage = "Degree Name is Required")]
        public string Degree { get; set; }
        [NotNull]
        [ForeignKey("Degrees")]
        [Display(Name = "Normalized Degree")]
        [Required(ErrorMessage = "Normalized Degree Name is Required")]
        public string NormalizedDegree { get; set; }
        public virtual Degrees Degrees { get; set; }
    }
}

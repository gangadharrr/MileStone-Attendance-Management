using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MileStone_Attendance_Management.Models
{
    [Index(nameof(Degree), IsUnique = true)]
    public class Degrees
    {
        [Display(Name = "Degree Name")]
        [Required(ErrorMessage = "Degree Name is Required")]
        public string Degree { get; set; }
        [Key]
        [Display(Name = "Normalized Degree")]
        [Required(ErrorMessage = "Normalized Degree Name is Required")]
        public string NormalizedDegree { get; set; }
        public ICollection<Branches> Branches { get; set; }
    }
}

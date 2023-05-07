using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MileStone_Attendance_Management.Models
{
    public class CsvFileModel
    {
        public string Degree { get; set; }
        public string Branch { get; set; }
        public string Batch { get;set; }
        public string CourseId { get;set; }
        public string CourseName { get;set; }
        public int RollNumber { get; set; }
        public string Name { get; set; }
        public string PresentStatus { get; set; }
        public string Email { get; set; }

    }
}

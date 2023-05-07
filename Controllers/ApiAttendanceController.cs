using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MileStone_Attendance_Management.Data;
using MileStone_Attendance_Management.Models;

namespace MileStone_Attendance_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiAttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApiAttendanceController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
     

        // GET: api/ApiAttendance/5
        [HttpGet("{id}")]
        //[Authorize(Roles ="Admin,Attender,Professor")]
        public async Task<ActionResult<IEnumerable<CsvFileModel>>> GetAttendance(int id)
        {
          if (_context.Attendance == null)
          {
              return NotFound();
          }
            var attendance = await _context.Attendance.Where(m => m.AttendanceId == id).ToListAsync();

            var attendanceHistory=_context.AttendanceHistory.Find(id);
            if (attendance == null && attendanceHistory==null)
            {
                return NotFound();
            }

            List<CsvFileModel> csvFile = new List<CsvFileModel>();
            var course=_context.CoursesAssigned.Where(m => m.NormalizedDegree == attendanceHistory.NormalizedDegree && m.NormalizedBranch == attendanceHistory.NormalizedBranch&&m.CourseId==attendanceHistory.CourseId).FirstOrDefault();
            var _year = (course.Semester & 1) == 1 ? ((course.Semester + 1) / 2) - 1 : ((course.Semester) / 2) - 1;
            var _batch = (DateTime.Now.Month / 6) == 0 ? DateTime.Now.Year - _year - 1 : DateTime.Now.Year - _year;
            var _endYear = _batch + _context.Branches.Where(m => m.NormalizedDegree == attendanceHistory.NormalizedDegree && m.NormalizedBranch == attendanceHistory.NormalizedBranch).Select(m => m.Duration).FirstOrDefault();
            foreach (var item in  attendance) 
            {
                CsvFileModel csvField = new CsvFileModel();
                csvField.Degree= attendanceHistory.NormalizedDegree;
                csvField.Branch= attendanceHistory.NormalizedBranch;
                csvField.Batch = $"{_batch}-{_endYear}";
                csvField.CourseId = attendanceHistory.CourseId;
                csvField.CourseName = _context.Courses.Find(attendanceHistory.CourseId).CourseName;
                csvField.RollNumber = item.RollNumber;
                csvField.Name = item.Name;
                csvField.Email = item.Email;
                csvField.PresentStatus = item.PresentStatus ? "PRESENT" : "ABSENT";
                csvFile.Add(csvField);
            }
            return csvFile;
        }

       
        private bool AttendanceExists(int id)
        {
            return (_context.Attendance?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

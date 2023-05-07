using System.Data;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MileStone_Attendance_Management.Data;
using MileStone_Attendance_Management.Models;
using Newtonsoft.Json;
using CsvHelper.Configuration;
using CsvHelper;
using System.Text;
using System.Globalization;

namespace MileStone_Attendance_Management.Controllers
{
    [Authorize(Roles ="Admin,Attender,Professor,Student")]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AttendanceController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: Attendance
        [Authorize(Roles = "Admin,Attender,Professor")]
        public async Task<IActionResult> Index()
        {
            try { 
            if (User.IsInRole("Professor"))
            {
                List<AttendanceHistory> attendanceHistories = new List<AttendanceHistory>();
                var _applicationDbContext = _context.AttendanceHistory.Include(a => a.Courses).Include(a => a.Degrees).ToList();
                var _professor = _context.Employees.Find(User.Identity?.Name);
                var coursesList = _context.SectionsAssigned.Where(m => m.Email == _professor.Email).ToList();
                foreach(var item in coursesList)
                {
                    attendanceHistories.AddRange(_applicationDbContext.Where(m=>m.NormalizedDegree==_professor.NormalizedDegree&&m.NormalizedBranch==_professor.NormalizedBranch&&m.CourseId==item.CourseId && m.Section==item.Section).ToList());
                }
                return View(attendanceHistories);

            }
            if (User.IsInRole("Attender"))
            {
                var _applicationDbContext = _context.AttendanceHistory.Include(a => a.Courses).Include(a => a.Degrees).ToList();
                var _attender = _context.Employees.Find(User.Identity?.Name);
                
                return View(_applicationDbContext.Where(m=>m.NormalizedDegree==_attender.NormalizedDegree&&m.NormalizedBranch==_attender.NormalizedBranch).ToList());
            }
            var applicationDbContext = _context.AttendanceHistory.Include(a => a.Courses).Include(a => a.Degrees);
            return View(await applicationDbContext.ToListAsync());
            }
            catch (Exception ex) 
            {
                return Problem(ex.Message);
            }
        }

        // GET: Attendance/Details/5
        [Authorize(Roles = "Admin,Attender,Professor")]
        public IActionResult Details(string CourseId, string Section, int id)
        {
            ViewBag.CourseId = CourseId;
            ViewBag.Section = Section;
            AttendanceHistory attendanceHistory = new AttendanceHistory();
            List<Attendance> attendances = new List<Attendance>();
            try { 
                attendanceHistory = _context.AttendanceHistory.Find(id);
                attendanceHistory.Degrees = _context.Degrees.Find(attendanceHistory.NormalizedDegree);
                attendanceHistory.Courses = _context.Courses.Find(attendanceHistory.CourseId);
                attendances = _context.Attendance.Where(m => m.AttendanceId == id).ToList();
            }
            catch(Exception e)
            {
                return Problem(e.Message);
            }
            ViewBag.AttendanceHistory = attendanceHistory;
            ViewBag.AttendanceList = attendances;
            return View(attendanceHistory);
        }

        // GET: Attendance/Create
        [Authorize(Roles = "Professor")]
        public IActionResult Create()
        {
            try { 
            var _professor = _context.Employees.Find(User.Identity?.Name);
            var _sectionsAssigned= _context.SectionsAssigned.Where(m=>m.Employees.NormalizedDegree==_professor.NormalizedDegree&&m.Employees.NormalizedBranch == _professor.NormalizedBranch &&m.Email==_professor.Email).ToList();
            ViewBag.SectionsAssigned = _sectionsAssigned;
            ViewBag.CoursesId= _sectionsAssigned.Select(m=>$"{m.CourseId}-{_context.Courses.Find(m.CourseId).CourseName}").Distinct().ToList(); 
            Dictionary<string,List<string>> _sectionsList= new Dictionary<string,List<string>>();  
            foreach (var section in _sectionsAssigned)
            {
                if (_sectionsList.ContainsKey(section.CourseId))
                {
                    _sectionsList[section.CourseId].Add(section.Section);
                }
                else{
                    _sectionsList.Add(section.CourseId, new List<string>());
                    _sectionsList[section.CourseId].Add(section.Section);

                }
            }
            ViewBag.SectionsList= _sectionsList;
            return View();
            }
            catch(Exception ex) { return Problem(ex.Message); }
        }

        // POST: Attendance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Professor")]
        public IActionResult Create(IFormCollection collection)
        {
            try { 
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(AttendanceSheet), new { CourseId = collection["CourseId"].ToString(), Section = collection["Section"].ToString() });
            }
            var _professor = _context.Employees.Find(User.Identity?.Name);
            var _sectionsAssigned = _context.SectionsAssigned.Where(m => m.Employees.NormalizedDegree == _professor.NormalizedDegree && m.Employees.NormalizedBranch == _professor.NormalizedBranch).ToList();
            ViewBag.SectionsAssigned = _sectionsAssigned;
            ViewBag.CoursesId = _sectionsAssigned.Select(m => $"{m.CourseId}-{_context.Courses.Find(m.CourseId).CourseName}").Distinct().ToList();
            Dictionary<string, List<string>> _sectionsList = new Dictionary<string, List<string>>();
            foreach (var section in _sectionsAssigned)
            {
                if (_sectionsList.ContainsKey(section.CourseId))
                {
                    _sectionsList[section.CourseId].Add(section.Section);
                }
                else
                {
                    _sectionsList.Add(section.CourseId, new List<string>());
                    _sectionsList[section.CourseId].Add(section.Section);

                }
            }
            ViewBag.SectionsList = _sectionsList;
            return View();
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }
        [Authorize(Roles = "Admin,Attender,Professor")]
        public async Task<IActionResult> AttendanceSheet(string CourseId,string Section,int id=0)
        {   
            ViewBag.CourseId = CourseId;
            ViewBag.Section = Section;
            AttendanceHistory attendanceHistory = new AttendanceHistory();
            List<Attendance> attendances = new List<Attendance>();
            try
            {
                var _professor = _context.Employees.Find(User.Identity?.Name);
                var course = _context.CoursesAssigned.Where(m => m.NormalizedDegree == _professor.NormalizedDegree && m.NormalizedBranch == _professor.NormalizedBranch && m.CourseId == CourseId).FirstOrDefault();
                var _year = (course.Semester & 1) == 1 ? ((course.Semester + 1) / 2) - 1 : ((course.Semester) / 2) - 1;
                var _batch = (DateTime.Now.Month / 6) == 0 ? DateTime.Now.Year - _year - 1 : DateTime.Now.Year - _year;
                var _endYear = _batch + _context.Branches.Where(m => m.NormalizedDegree == _professor.NormalizedDegree && m.NormalizedBranch == _professor.NormalizedBranch).Select(m => m.Duration).FirstOrDefault();
         
            
                if (_context.AttendanceHistory.Find(id==0?attendanceHistory.AttendanceId:id) == null) 
                {
                    attendanceHistory.NormalizedDegree = _professor.NormalizedDegree;
                    attendanceHistory.NormalizedBranch = _professor.NormalizedBranch;
                    attendanceHistory.CourseId= CourseId;
                    attendanceHistory.Section = Section;
                    attendanceHistory.Batch = $"{_batch}-{_endYear}";
                    attendanceHistory.TimeStamp= DateTime.Now;
                    attendanceHistory.Degrees = _context.Degrees.Find(attendanceHistory.NormalizedDegree);
                    attendanceHistory.Courses=_context.Courses.Find(attendanceHistory.CourseId);
                    _context.Add(attendanceHistory);
                    await _context.SaveChangesAsync();
                
                    foreach(var student in _context.Students.Where(m=>m.NormalizedDegree==_professor.NormalizedDegree&&m.NormalizedBranch==_professor.NormalizedBranch&&m.Batch==$"{_batch}-{_endYear}"&&m.Section==Section).ToList())
                    {
                        Attendance _attendance=new Attendance();
                        _attendance.AttendanceId=attendanceHistory.AttendanceId;
                        _attendance.RollNumber = student.RollNumber;
                        _attendance.Name=student.Name;
                        _attendance.Email=student.Email;
                        _attendance.Students = student;
                        _attendance.AttendanceHistory=attendanceHistory;
                        attendances.Add(_attendance);
                        List<Attendance> studentrow=_context.Attendance.Where(m => m.AttendanceId == _attendance.AttendanceId && m.RollNumber == _attendance.RollNumber).ToList();
                        if (studentrow.Count==0) 
                        {
                            _context.Add(_attendance);
                            _context.SaveChanges();
                        }
                    }
                }
                else
                {
                    attendanceHistory = _context.AttendanceHistory.Find(id);
                    attendanceHistory.Degrees = _context.Degrees.Find(attendanceHistory.NormalizedDegree);
                    attendanceHistory.Courses = _context.Courses.Find(attendanceHistory.CourseId);
                    attendances = _context.Attendance.Where(m => m.AttendanceId == id).ToList();
                }
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
            ViewBag.AttendanceHistory = attendanceHistory;
            ViewBag.AttendanceList = attendances;
            return View(attendanceHistory);
        }
        [Authorize(Roles = "Admin,Attender,Professor")]
        public IActionResult AttendanceSheets(int Id,string CourseId, string Section)
        {
            var item = _context.Attendance.Find(Id);
            if (item != null)
            { 
                item.PresentStatus = !item.PresentStatus;
                _context.Update(item);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(AttendanceSheet), new { CourseId=CourseId,Section=Section,id=item.AttendanceId});
        }
        // GET: Attendance/Delete/5
        [Authorize(Roles="Admin,Attender")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AttendanceHistory == null)
            {
                return NotFound();
            }

            var attendanceHistory = await _context.AttendanceHistory
                .Include(a => a.Courses)
                .Include(a => a.Degrees)
                .FirstOrDefaultAsync(m => m.AttendanceId == id);
            if (attendanceHistory == null)
            {
                return NotFound();
            }

            return View(attendanceHistory);
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Attender")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AttendanceHistory == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AttendanceHistory'  is null.");
            }
            var attendanceHistory = await _context.AttendanceHistory.FindAsync(id);
            if (attendanceHistory != null)
            {
                _context.AttendanceHistory.Remove(attendanceHistory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Student")]
        public  IActionResult StudentAttendance()
        {
            try { 
            var _student = _context.Students.Find(User.Identity?.Name);
            var attendance = _context.Attendance.Where(m => m.RollNumber == _student.RollNumber).ToList();
            foreach(var item in attendance) 
            {
                item.AttendanceHistory=_context.AttendanceHistory.Find(item.AttendanceId);
                item.AttendanceHistory.Courses=_context.Courses.Find(item.AttendanceHistory.CourseId);
            }
            return View(attendance);
            }catch (Exception ex) { return Problem(ex.Message); }
        }
        [Authorize(Roles = "Admin,Attender,Professor")]
        public async Task<FileResult> Download(int id)
        {
            string Baseurl = "https://localhost:7214/";
            string url = $"api/ApiAttendance/{id}";

            string title = "output";
            string text = "Download Failed";
            byte[] byteArray = Encoding.ASCII.GetBytes(text);
            MemoryStream stream = new MemoryStream(byteArray);
            try { 
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var Res = await client.GetAsync(url);
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    //var responseJSON = Res.Content.ReadAsStream();
                    
                    var model = JsonConvert.DeserializeObject<IEnumerable<CsvFileModel>>(response);
                    var attendanceHistory = _context.AttendanceHistory.Find(id);
                    title = $"{attendanceHistory.NormalizedDegree}-{attendanceHistory.NormalizedBranch}-{attendanceHistory.Section}-{attendanceHistory.CourseId}-({attendanceHistory.TimeStamp})";
                    text = jsonToCSV(response);
                    byteArray = Encoding.ASCII.GetBytes(text);
                    stream = new MemoryStream(byteArray);

                    return File(stream, "text/csv", $"{title}.csv");
                }
            }
            }
            catch { return File(stream, "text/csv", $"{title}.csv"); }
            
            return File(stream, "text/csv", $"{title}.csv"); 
        }
        public static DataTable jsonStringToTable(string jsonContent)
        {
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonContent);
            return dt;
        }
        public static string jsonToCSV(string jsonContent)
        {
            StringWriter csvString = new StringWriter();
            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = false
            };
            using (var csv = new CsvWriter(csvString, csvConfig))
            {
          

                using (var dt = jsonStringToTable(jsonContent))
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        csv.WriteField(column.ColumnName);
                    }
                    csv.NextRecord();

                    foreach (DataRow row in dt.Rows)
                    {
                        for (var i = 0; i < dt.Columns.Count; i++)
                        {
                            csv.WriteField(row[i]);
                        }
                        csv.NextRecord();
                    }
                }
            }
            return csvString.ToString();
        }
        private bool AttendanceHistoryExists(int id)
        {
          return (_context.AttendanceHistory?.Any(e => e.AttendanceId == id)).GetValueOrDefault();
        }
    }
}

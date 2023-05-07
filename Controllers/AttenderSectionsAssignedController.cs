using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MileStone_Attendance_Management.Data;
using MileStone_Attendance_Management.Models;

namespace MileStone_Attendance_Management.Controllers
{
    [Authorize(Roles = "Attender")]
    public class AttenderSectionsAssignedController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public AttenderSectionsAssignedController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
           
        }

        // GET: AttenderSectionsAssigned
        public async Task<IActionResult> Index()
        {
            Employees _Attender = _context.Employees.Find(User.Identity.Name);
            var applicationDbContext =  _context.SectionsAssigned.Include(s => s.Courses).Include(s => s.Employees);
            return View(await applicationDbContext.Where(m => m.Employees.NormalizedDegree == _Attender.NormalizedDegree && m.Employees.NormalizedBranch == _Attender.NormalizedBranch).ToListAsync());
        }

        // GET: AttenderSectionsAssigned/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SectionsAssigned == null)
            {
                return NotFound();
            }

            var sectionsAssigned = await _context.SectionsAssigned
                .Include(s => s.Courses)
                .Include(s => s.Employees)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sectionsAssigned == null)
            {
                return NotFound();
            }

            return View(sectionsAssigned);
        }

        // GET: AttenderSectionsAssigned/Create
        public IActionResult Create()
        {

            Dictionary<string, List<string>> _sectionsList = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> _coursesList = new Dictionary<string, List<string>>();
            Employees _Attender = _context.Employees.Find(User.Identity.Name);
            foreach (var employee in _context.Employees.Where(m => m.NormalizedDegree == _Attender.NormalizedDegree && m.Roles.Name == "Professor").ToList())
            {
                foreach (var course in _context.CoursesAssigned.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch).ToList())
                {
                    var _year = (course.Semester & 1) == 1 ? ((course.Semester + 1) / 2) - 1 : ((course.Semester) / 2) - 1;
                    var _batch = (DateTime.Now.Month / 6) == 0 ? DateTime.Now.Year - _year - 1 : DateTime.Now.Year - _year;
                    var _endYear = _batch + _context.Branches.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch).Select(m => m.Duration).FirstOrDefault();
                    _sectionsList.Add($"{employee.Email}{course.CourseId}", _context.Students.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch && m.Batch == $"{_batch}-{_endYear}").Select(m => m.Section).Distinct().ToList());

                }
                _coursesList.Add($"{employee.Email}", _context.CoursesAssigned.Where(m => m.NormalizedDegree == employee.NormalizedDegree &&
                                        m.NormalizedBranch == employee.NormalizedBranch && (m.Semester & 1) == DateTime.Now.Month / 6).Select(m => m.CourseId + "-" + m.Courses.CourseName).ToList());

            }
            ViewBag.SectionsList = _sectionsList;
            ViewBag.CoursesList = _coursesList;
            ViewBag.Email = _context.Employees.Where(m => m.NormalizedDegree==_Attender.NormalizedDegree && m.NormalizedBranch==_Attender.NormalizedBranch && m.Roles.Name == "Professor").ToList();
            return View();
        }

        // POST: AttenderSectionsAssigned/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,CourseId,Section,NormalizedDegree,NormalizedBranch")] SectionsAssigned sectionsAssigned)
        {
            Dictionary<string, List<string>> _sectionsList = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> _coursesList = new Dictionary<string, List<string>>();
            Employees _Attender = _context.Employees.Find(User.Identity.Name);
            foreach (var employee in _context.Employees.Where(m => m.NormalizedDegree == _Attender.NormalizedDegree && m.Roles.Name == "Professor").ToList())
            {
                foreach (var course in _context.CoursesAssigned.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch).ToList())
                {
                    var _year = (course.Semester & 1) == 1 ? ((course.Semester + 1) / 2) - 1 : ((course.Semester) / 2) - 1;
                    var _batch = (DateTime.Now.Month / 6) == 0 ? DateTime.Now.Year - _year - 1 : DateTime.Now.Year - _year;
                    var _endYear = _batch + _context.Branches.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch).Select(m => m.Duration).FirstOrDefault();
                    _sectionsList.Add($"{employee.Email}{course.CourseId}", _context.Students.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch && m.Batch == $"{_batch}-{_endYear}").Select(m => m.Section).Distinct().ToList());

                }
                _coursesList.Add($"{employee.Email}", _context.CoursesAssigned.Where(m => m.NormalizedDegree == employee.NormalizedDegree &&
                                        m.NormalizedBranch == employee.NormalizedBranch && (m.Semester & 1) == DateTime.Now.Month / 6).Select(m => m.CourseId + "-" + m.Courses.CourseName).ToList());

            }

            ViewBag.SectionsList = _sectionsList;
            if (ModelState.IsValid)
            {
                _context.Add(sectionsAssigned);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CoursesList = _coursesList;
            ViewBag.Email = _context.Employees.Where(m => m.NormalizedDegree == _Attender.NormalizedDegree && m.NormalizedBranch == _Attender.NormalizedBranch && m.Roles.Name == "Professor").ToList();

            return View(sectionsAssigned);
        }

        // GET: AttenderSectionsAssigned/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Dictionary<string, List<string>> _sectionsList = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> _coursesList = new Dictionary<string, List<string>>();
            Employees _Attender = _context.Employees.Find(User.Identity.Name);
            foreach (var employee in _context.Employees.Where(m => m.NormalizedDegree == _Attender.NormalizedDegree && m.Roles.Name == "Professor").ToList())
            {
                foreach (var course in _context.CoursesAssigned.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch).ToList())
                {
                    var _year = (course.Semester & 1) == 1 ? ((course.Semester + 1) / 2) - 1 : ((course.Semester) / 2) - 1;
                    var _batch = (DateTime.Now.Month / 6) == 0 ? DateTime.Now.Year - _year - 1 : DateTime.Now.Year - _year;
                    var _endYear = _batch + _context.Branches.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch).Select(m => m.Duration).FirstOrDefault();
                    _sectionsList.Add($"{employee.Email}{course.CourseId}", _context.Students.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch && m.Batch == $"{_batch}-{_endYear}").Select(m => m.Section).Distinct().ToList());

                }
                _coursesList.Add($"{employee.Email}", _context.CoursesAssigned.Where(m => m.NormalizedDegree == employee.NormalizedDegree &&
                                        m.NormalizedBranch == employee.NormalizedBranch && (m.Semester & 1) == DateTime.Now.Month / 6).Select(m => m.CourseId + "-" + m.Courses.CourseName).ToList());

            }
            if (id == null || _context.SectionsAssigned == null)
            {
                return NotFound();
            }

            var sectionsAssigned = await _context.SectionsAssigned.FindAsync(id);
            if (sectionsAssigned == null)
            {
                return NotFound();
            }
            ViewBag.CoursesList = _coursesList;
            ViewBag.Email = _context.Employees.Where(m => m.NormalizedDegree == _Attender.NormalizedDegree && m.NormalizedBranch == _Attender.NormalizedBranch && m.Roles.Name == "Professor").ToList();

            return View(sectionsAssigned);
        }

        // POST: AttenderSectionsAssigned/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,CourseId,Section,NormalizedDegree,NormalizedBranch")] SectionsAssigned sectionsAssigned)
        {
            Dictionary<string, List<string>> _sectionsList = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> _coursesList = new Dictionary<string, List<string>>();
            Employees _Attender = _context.Employees.Find(User.Identity.Name);
            foreach (var employee in _context.Employees.Where(m => m.NormalizedDegree == _Attender.NormalizedDegree && m.Roles.Name == "Professor").ToList())
            {
                foreach (var course in _context.CoursesAssigned.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch).ToList())
                {
                    var _year = (course.Semester & 1) == 1 ? ((course.Semester + 1) / 2) - 1 : ((course.Semester) / 2) - 1;
                    var _batch = (DateTime.Now.Month / 6) == 0 ? DateTime.Now.Year - _year - 1 : DateTime.Now.Year - _year;
                    var _endYear = _batch + _context.Branches.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch).Select(m => m.Duration).FirstOrDefault();
                    _sectionsList.Add($"{employee.Email}{course.CourseId}", _context.Students.Where(m => m.NormalizedDegree == employee.NormalizedDegree && m.NormalizedBranch == employee.NormalizedBranch && m.Batch == $"{_batch}-{_endYear}").Select(m => m.Section).Distinct().ToList());

                }
                _coursesList.Add($"{employee.Email}", _context.CoursesAssigned.Where(m => m.NormalizedDegree == employee.NormalizedDegree &&
                                        m.NormalizedBranch == employee.NormalizedBranch && (m.Semester & 1) == DateTime.Now.Month / 6).Select(m => m.CourseId + "-" + m.Courses.CourseName).ToList());

            }
            if (id != sectionsAssigned.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sectionsAssigned);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectionsAssignedExists(sectionsAssigned.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CoursesList = _coursesList;
            ViewBag.Email = _context.Employees.Where(m => m.NormalizedDegree == _Attender.NormalizedDegree && m.NormalizedBranch == _Attender.NormalizedBranch && m.Roles.Name == "Professor").ToList();
            return View(sectionsAssigned);
        }

       
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.SectionsAssigned == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SectionsAssigned'  is null.");
            }
            var sectionsAssigned = await _context.SectionsAssigned.FindAsync(id);
            if (sectionsAssigned != null)
            {
                _context.SectionsAssigned.Remove(sectionsAssigned);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult DeleteAll()
        {
            foreach (var item in Directory.GetFiles(@"wwwroot\Files\"))
            {
                string filepath = Path.Combine(Directory.GetCurrentDirectory(), item);
                System.IO.File.Delete(filepath);
            }
            if (_context.SectionsAssigned == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SectionsAssigned'  is null.");
            }
            var sections = _context.SectionsAssigned != null ?
                           _context.SectionsAssigned.ToList() :
                          new List<SectionsAssigned>();
            if (sections != null)
            {
                try
                {
                    foreach (var item in sections)
                    {
                        _context.SectionsAssigned.Remove(item);
                        _context.SaveChanges();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DBConcurrencyException ex)
                {
                    return Problem(ex.Message);

                }
                catch (Exception ex)
                {
                    return Problem(ex.Message);
                }

            }
            return RedirectToAction(nameof(Index));

        }
        private bool SectionsAssignedExists(int id)
        {
          return (_context.SectionsAssigned?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

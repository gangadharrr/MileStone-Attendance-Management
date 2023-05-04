using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MileStone_Attendance_Management.Data;
using MileStone_Attendance_Management.Models;
using System.Data;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MileStone_Attendance_Management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoursesAssignedController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CoursesAssignedController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CoursesAssigned
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CoursesAssigned.Include(c => c.Courses).Include(c => c.Degrees);
            return View(await applicationDbContext.ToListAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IFormFile UploadedFile)
        {
            if (Request != null)
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files\", UploadedFile.FileName);
                if(System.IO.File.Exists(filePath))
                {
                    
                    return Problem("This File is already Added Try another file to avoid duplicates");
                }
                if (Request.Form.Files.Count != 0 && !string.IsNullOrEmpty(UploadedFile.FileName))
                {
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        UploadedFile.CopyTo(stream);
                    }
                    var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
                    {
                        HasHeaderRecord = false
                    };
                    using var streamReader = System.IO.File.OpenText(filePath);
                    using var csvReader = new CsvReader(streamReader, csvConfig);
                    List<CoursesAssigned> courseList = new List<CoursesAssigned>();
                    bool _firstEntryLoop = true;
                    while (csvReader.Read())
                    {
                        if (_firstEntryLoop)
                        {
                            try { 
                                _firstEntryLoop = false;
                                var course = new CoursesAssigned();
                                course.CourseId = csvReader.GetField<string>(0);
                                course.NormalizedDegree = csvReader.GetField<string>(4);
                                course.NormalizedBranch = csvReader.GetField<string>(5);
                                if (course.CourseId == "CourseId"  && course.NormalizedDegree == "NormalizedDegree"&&course.NormalizedBranch == "NormalizedBranch")
                                {
                                    continue;
                                }
                                else
                                {
                                    streamReader.Close();
                                    System.IO.File.Delete(filePath);
                                    return Problem("Incorrect Format of Fields Try Again");
                                }
                            }
                            catch(Exception ex)
                            {
                                return Problem(ex.Message);

                            }
                        }
                        else
                        {
                            var course = new CoursesAssigned();
                            course.CourseId = csvReader.GetField<string>(0);
                            course.Semester = Convert.ToInt32(csvReader.GetField<string>(3));
                            course.NormalizedDegree = csvReader.GetField<string>(4);
                            course.NormalizedBranch = csvReader.GetField<string>(5);
                            courseList.Add(course);
                        }
                    }
                    try
                    {
                        foreach (var course in courseList)
                        {
                            _context.Add(course);
                        }
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        streamReader.Close();
                        System.IO.File.Delete(filePath);
                        return Problem("Forien Key error Check the courses, Degrees and Branches"+e.Message);
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        // GET: CoursesAssigned/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CoursesAssigned == null)
            {
                return NotFound();
            }

            var coursesAssigned = await _context.CoursesAssigned
                .Include(c => c.Courses)
                .Include(c => c.Degrees)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coursesAssigned == null)
            {
                return NotFound();
            }

            return View(coursesAssigned);
        }

        // GET: CoursesAssigned/Create
        public IActionResult Create()
        {
            
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId");
            ViewData["CourseIds"] = _context.Courses.ToList();
            Dictionary<string, Courses> _coursesList = new Dictionary<string, Courses>();
            foreach (var c in _context.Courses.ToList())
            {
                _coursesList.Add(c.CourseId,c);
            }
            ViewBag.Courses = _coursesList;
            List<string> BranchNdDegree = new List<string>();
            Dictionary<string,List<int>> _durationList= new Dictionary<string,List<int>>();
            foreach(var item in _context.Branches.ToList())
            {
                BranchNdDegree.Add($"{item.NormalizedDegree}$@{item.NormalizedBranch}");
                if(!_durationList.ContainsKey($"{item.NormalizedDegree}$@{item.NormalizedBranch}"))
                {
                    List<int> _duration = new List<int>();
                    for(int i=1;i<=2*item.Duration;i++)
                    {
                        _duration.Add(i);
                    }
                    _durationList.Add($"{item.NormalizedDegree}$@{item.NormalizedBranch}",_duration);
                }
            }
            ViewBag.Duration= _durationList;
            ViewData["BranchNdDegree"] = BranchNdDegree.Distinct().ToList();    


            return View();
        }

        // POST: CoursesAssigned/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            CoursesAssigned coursesAssigned=new CoursesAssigned();
            ViewData["CourseIds"] = _context.Courses.ToList();
            List<string> BranchNdDegree = new List<string>();
            foreach (var item in _context.Branches.ToList())
            {
                BranchNdDegree.Add($"{item.NormalizedDegree}$@{item.NormalizedBranch}");
            }
            ViewData["BranchNdDegree"] = BranchNdDegree.Distinct().ToList();
            Dictionary<string, List<int>> _durationList = new Dictionary<string, List<int>>();
            foreach (var item in _context.Branches.ToList())
            {
                BranchNdDegree.Add($"{item.NormalizedDegree}$@{item.NormalizedBranch}");
                if (!_durationList.ContainsKey($"{item.NormalizedDegree}$@{item.NormalizedBranch}"))
                {
                    List<int> _duration = new List<int>();
                    for (int i = 1; i <= 2 * item.Duration; i++)
                    {
                        _duration.Add(i);
                    }
                    _durationList.Add($"{item.NormalizedDegree}$@{item.NormalizedBranch}", _duration);
                }
            }
            ViewBag.Duration = _durationList;
            Dictionary<string, Courses> _coursesList = new Dictionary<string, Courses>();
            foreach (var c in _context.Courses.ToList())
            {
                _coursesList.Add(c.CourseId, c);
            }
            ViewBag.Courses = _coursesList;
            var _strings= collection["SelectedBranchNdDegree"].ToString().Split("$@");
            coursesAssigned.NormalizedDegree = _strings[0];
            coursesAssigned.NormalizedBranch = _strings[1];
            coursesAssigned.CourseId = collection["SelectedCourseId"].ToString();
            coursesAssigned.Semester = Convert.ToInt32(collection["Semester"].ToString());
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(coursesAssigned);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch(Exception e)
                {
                    Problem(e.Message);
                }
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", coursesAssigned.CourseId);
            ViewData["NormalizedDegree"] = new SelectList(_context.Degrees, "NormalizedDegree", "NormalizedDegree", coursesAssigned.NormalizedDegree);
            return View(coursesAssigned);
        }

        // GET: CoursesAssigned/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CoursesAssigned == null)
            {
                return NotFound();
            }
            ViewBag.Id= id;
            var coursesAssigned = await _context.CoursesAssigned.FindAsync(id);
            if(coursesAssigned == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId");
            ViewData["CourseIds"] = _context.Courses.ToList();
            Dictionary<string, Courses> _coursesList = new Dictionary<string, Courses>();
            foreach (var c in _context.Courses.ToList())
            {
                _coursesList.Add(c.CourseId, c);
            }
            ViewBag.SelectedBranchNdDegree = $"{coursesAssigned.NormalizedDegree}$@{coursesAssigned.NormalizedBranch}";
            ViewBag.Courses = _coursesList;
            List<string> BranchNdDegree = new List<string>();
            foreach (var item in _context.Branches.ToList())
            {
                BranchNdDegree.Add($"{item.NormalizedDegree}$@{item.NormalizedBranch}");
            }
            ViewData["BranchNdDegree"] = BranchNdDegree.Distinct().ToList();
            Dictionary<string, List<int>> _durationList = new Dictionary<string, List<int>>();
            foreach (var item in _context.Branches.ToList())
            {
                BranchNdDegree.Add($"{item.NormalizedDegree}$@{item.NormalizedBranch}");
                if (!_durationList.ContainsKey($"{item.NormalizedDegree}$@{item.NormalizedBranch}"))
                {
                    List<int> _duration = new List<int>();
                    for (int i = 1; i <= 2 * item.Duration; i++)
                    {
                        _duration.Add(i);
                    }
                    _durationList.Add($"{item.NormalizedDegree}$@{item.NormalizedBranch}", _duration);
                }
            }
            ViewBag.Duration = _durationList;
            return View(coursesAssigned);
        }

        // POST: CoursesAssigned/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,IFormCollection collection)
        {
            var coursesAssigned = _context.CoursesAssigned.Find(id);
            if (coursesAssigned !=null && id != coursesAssigned.Id)
            {
                return NotFound();
            }
            ViewData["CourseIds"] = _context.Courses.ToList();
            List<string> BranchNdDegree = new List<string>();
            foreach (var item in _context.Branches.ToList())
            {
                BranchNdDegree.Add($"{item.NormalizedDegree}$@{item.NormalizedBranch}");
            }
            ViewData["BranchNdDegree"] = BranchNdDegree.Distinct().ToList();
            Dictionary<string, List<int>> _durationList = new Dictionary<string, List<int>>();
            foreach (var item in _context.Branches.ToList())
            {
                BranchNdDegree.Add($"{item.NormalizedDegree}$@{item.NormalizedBranch}");
                if (!_durationList.ContainsKey($"{item.NormalizedDegree}$@{item.NormalizedBranch}"))
                {
                    List<int> _duration = new List<int>();
                    for (int i = 1; i <= 2 * item.Duration; i++)
                    {
                        _duration.Add(i);
                    }
                    _durationList.Add($"{item.NormalizedDegree}$@{item.NormalizedBranch}", _duration);
                }
            }
            ViewBag.Duration = _durationList;
            ViewBag.SelectedBranchNdDegree = coursesAssigned.NormalizedDegree + "$@" + coursesAssigned.NormalizedBranch;
            Dictionary<string, Courses> _coursesList = new Dictionary<string, Courses>();
            foreach (var c in _context.Courses.ToList())
            {
                _coursesList.Add(c.CourseId, c);
            }
            ViewBag.Courses = _coursesList;
            Console.WriteLine($"<--------------{collection["SelectedBranchNdDegree"]}------------>");
            var _strings = collection["SelectedBranchNdDegree"].ToString().Split("$@");
            coursesAssigned.NormalizedDegree = _strings[0];
            coursesAssigned.NormalizedBranch = _strings[1];
            coursesAssigned.CourseId = collection["SelectedCourseId"].ToString();
            Console.WriteLine($"<--------------{collection["Semester"]}------------>");
            coursesAssigned.Semester = Convert.ToInt32(collection["Semester"].ToString());
            //if (ModelState.IsValid)
            
                try
                {
                    _context.Update(coursesAssigned);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoursesAssignedExists(coursesAssigned.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                    Problem("DbUpdateConcurrencyException");
                    }
                }
                catch(Exception ex)
                {
                    return Problem(ex.Message);
                }
            
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", coursesAssigned.CourseId);
            ViewData["NormalizedDegree"] = new SelectList(_context.Degrees, "NormalizedDegree", "NormalizedDegree", coursesAssigned.NormalizedDegree);
            return View(coursesAssigned);
        }

      
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.CoursesAssigned == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CoursesAssigned'  is null.");
            }
            var coursesAssigned = await _context.CoursesAssigned.FindAsync(id);
            if (coursesAssigned != null)
            {
                _context.CoursesAssigned.Remove(coursesAssigned);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult DeleteAll()
        {
            foreach (var item in Directory.GetFiles(@"wwwroot\Files\"))
            {
                string filepath=Path.Combine(Directory.GetCurrentDirectory(), item);
                System.IO.File.Delete(filepath);
            }
            if (_context.CoursesAssigned == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CoursesAssigned'  is null.");
            }
            var courses = _context.CoursesAssigned != null ?
                           _context.CoursesAssigned.ToList() :
                          new List<CoursesAssigned>();
            if (courses != null)
            {
                try
                {
                    foreach (var item in courses)
                    {
                        _context.CoursesAssigned.Remove(item);
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
        private bool CoursesAssignedExists(int id)
        {
          return (_context.CoursesAssigned?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

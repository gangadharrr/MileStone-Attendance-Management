using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MileStone_Attendance_Management.Data;
using MileStone_Attendance_Management.Models;

namespace MileStone_Attendance_Management.Controllers
{
    [Authorize(Roles ="Admin")]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Students.Include(s => s.Degrees);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var students = await _context.Students
                .Include(s => s.Degrees)
                .FirstOrDefaultAsync(m => m.Email == id);
            if (students == null)
            {
                return NotFound();
            }

            return View(students);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewBag.Sections = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            Dictionary<string,List<string>> _branchesList=new Dictionary<string,List<string>>();
            foreach (var degree in _context.Degrees.ToList())
            {
                _branchesList.Add(degree.NormalizedDegree,_context.Branches.Where(m => m.NormalizedDegree == degree.NormalizedDegree).Select(m => m.NormalizedBranch).ToList());
                foreach(var  item in _branchesList[degree.NormalizedDegree])
                {
                    Console.WriteLine(item);
                }
            }
            ViewBag.BranchesList = _branchesList;
            Dictionary<string,List<string>> _batchList=new Dictionary<string,List<string>>();
            foreach (var branch in _context.Branches.ToList())
            {
                List<string> _dateString = new List<string>();
                var dateOnly = DateTime.Now.Year;
                for (int i=dateOnly - branch.Duration+1; i <= dateOnly;i++)
                {
                    _dateString.Add($"{i}-{i+branch.Duration}");
                }
                _batchList.Add(branch.NormalizedDegree+branch.NormalizedBranch, _dateString);
            }
            ViewBag.BranchesList = _branchesList;
            ViewBag.BatchList = _batchList;
            ViewBag.NormalizedDegree = _context.Degrees.Select(m => m.NormalizedDegree).ToList();
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            Students students = new Students();
            students.Name = collection["Name"].ToString();
            students.Email = collection["Email"].ToString();
            students.RollNumber = Convert.ToInt32(collection["RollNumber"].ToString());
            students.Section = collection["Section"].ToString();
            students.NormalizedDegree = collection["NormalizedDegree"].ToString();
            students.NormalizedBranch = collection["NormalizedBranch"].ToString();
            students.Batch = collection["Batch"].ToString();
            if (ModelState.IsValid)
            {
                _context.Add(students);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Sections = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            Dictionary<string, List<string>> _branchesList = new Dictionary<string, List<string>>();
            foreach (var degree in _context.Degrees.ToList())
            {
                _branchesList.Add(degree.NormalizedDegree, _context.Branches.Where(m => m.NormalizedDegree == degree.NormalizedDegree).Select(m => m.NormalizedBranch).ToList());
                foreach (var item in _branchesList[degree.NormalizedDegree])
                {
                    Console.WriteLine(item);
                }
            }
            ViewBag.BranchesList = _branchesList;
            Dictionary<string, List<string>> _batchList = new Dictionary<string, List<string>>();
            foreach (var branch in _context.Branches.ToList())
            {
                List<string> _dateString = new List<string>();
                var dateOnly = DateTime.Now.Year;
                for (int i = dateOnly - branch.Duration + 1; i <= dateOnly; i++)
                {
                    _dateString.Add($"{i}-{i + branch.Duration}");
                }
                _batchList.Add(branch.NormalizedDegree + branch.NormalizedBranch, _dateString);
            }
            ViewBag.BranchesList = _branchesList;
            ViewBag.BatchList = _batchList;
            ViewBag.NormalizedDegree = _context.Degrees.Select(m => m.NormalizedDegree).ToList();
            return View(students);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var students = await _context.Students.FindAsync(id);
            if (students == null)
            {
                return NotFound();
            }
            ViewData["NormalizedDegree"] = new SelectList(_context.Degrees, "NormalizedDegree", "NormalizedDegree", students.NormalizedDegree);
            return View(students);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Email,Batch,Section,RollNumber,NormalizedDegree,NormalizedBranch")] Students students)
        {
            if (id != students.Email)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(students);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentsExists(students.Email))
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
            ViewData["NormalizedDegree"] = new SelectList(_context.Degrees, "NormalizedDegree", "NormalizedDegree", students.NormalizedDegree);
            return View(students);
        }

        

   
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Students'  is null.");
            }
            var students = await _context.Students.FindAsync(id);
            if (students != null)
            {
                _context.Students.Remove(students);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentsExists(string id)
        {
          return (_context.Students?.Any(e => e.Email == id)).GetValueOrDefault();
        }
    }
}

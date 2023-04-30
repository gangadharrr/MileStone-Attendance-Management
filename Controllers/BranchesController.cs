using System;
using System.Collections.Generic;
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
    [Authorize(Roles ="Admin")]
    public class BranchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public BranchesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Branches
        public async Task<IActionResult> Index()
        {
              return _context.Branches != null ? 
                          View(await _context.Branches.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Branches'  is null.");
        }

        // GET: Branches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Branches == null)
            {
                return NotFound();
            }

            var branches = await _context.Branches
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branches == null)
            {
                return NotFound();
            }

            return View(branches);
        }

        // GET: Branches/Create
        public IActionResult Create()
        {
            ViewBag.DegreeList = _context.Degrees != null ?
                          _context.Degrees.ToList() : new List<Degrees>();
            return _context.Branches != null ?
                          View(_context.Branches.ToList()) :
                          Problem("Entity set 'ApplicationDbContext.Branches'  is null.");
        }

        // POST: Branches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Branch,NormalizedBranch,NormalizedDegree")] Branches branches)
        {
            //if (ModelState.IsValid)
            var degree = _context.Degrees.Find(branches.NormalizedDegree);
            branches.Degree = degree.Degree;
            try
            {
                _context.Add(branches);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            return _context.Branches != null ?
                          View(_context.Branches.ToList()) :
                          Problem("Entity set 'ApplicationDbContext.Branches'  is null.");
        }

        // GET: Branches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Id = id;
            ViewBag.DegreeList = _context.Degrees != null ?
                          _context.Degrees.ToList() : new List<Degrees>();
            if (id == null || _context.Branches == null)
            {
                return NotFound();
            }

            var branches = await _context.Branches.FindAsync(id);
            if (branches == null)
            {
                return NotFound();
            }
            return _context.Branches != null ?
                          View(await _context.Branches.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Branches'  is null.");
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection collection)
        {
            Branches branches=new Branches();
            branches.Id = id;
            if (id != branches.Id)
            {
                return NotFound();
            }
            var degree = _context.Degrees.Find(collection["NormalizedDegree"]);
            branches.Degree = degree.Degree;
            branches.Branch = collection["Branch"];
            branches.NormalizedDegree = collection["NormalizedDegree"];
            branches.NormalizedBranch = collection["NormalizedBranch"];
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(branches);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchesExists(branches.Id))
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
            return View(branches);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Branches == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Branches'  is null.");
            }
            var branches = await _context.Branches.FindAsync(id);
            if (branches != null)
            {
                _context.Branches.Remove(branches);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BranchesExists(int id)
        {
          return (_context.Branches?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

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
    public class DegreesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DegreesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Degrees
        public async Task<IActionResult> Index()
        {
              return _context.Degrees != null ? 
                          View(await _context.Degrees.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Degrees'  is null.");
        }

        // GET: Degrees/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Degrees == null)
            {
                return NotFound();
            }

            var degrees = await _context.Degrees
                .FirstOrDefaultAsync(m => m.NormalizedDegree == id);
            if (degrees == null)
            {
                return NotFound();
            }

            return View(degrees);
        }

        // GET: Degrees/Create
        public IActionResult Create()
        {
            return _context.Degrees != null ?
                          View(_context.Degrees.ToList()) :
                          Problem("Entity set 'ApplicationDbContext.Degrees'  is null.");
        }

        // POST: Degrees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Degree,NormalizedDegree")] Degrees degrees)
        {
            if (ModelState.IsValid)
            {
                _context.Add(degrees);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(degrees);
        }

        // GET: Degrees/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.Id= id;
            if (id == null || _context.Degrees == null)
            {
                return NotFound();
            }

            var degrees = await _context.Degrees.FindAsync(id);
            if (degrees == null)
            {
                return NotFound();
            }
            return _context.Degrees != null ?
                          View(await _context.Degrees.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Degrees'  is null.");
        }

        // POST: Degrees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string Degree)
        {
            var degrees = await _context.Degrees.FindAsync(id);
            if (degrees == null)
            {
                return NotFound();
            }
            degrees.Degree = Degree;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(degrees);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DegreesExists(degrees.NormalizedDegree))
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
            return View(degrees);
        }

      
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.Degrees == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Degrees'  is null.");
            }
            var degrees = await _context.Degrees.FindAsync(id);
            if (degrees != null)
            {
                _context.Degrees.Remove(degrees);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DegreesExists(string id)
        {
          return (_context.Degrees?.Any(e => e.NormalizedDegree == id)).GetValueOrDefault();
        }
    }
}

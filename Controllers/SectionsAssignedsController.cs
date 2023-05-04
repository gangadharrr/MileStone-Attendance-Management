using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MileStone_Attendance_Management.Data;
using MileStone_Attendance_Management.Models;

namespace MileStone_Attendance_Management.Controllers
{
    public class SectionsAssignedsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SectionsAssignedsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SectionsAssigneds
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SectionsAssigned.Include(s => s.Courses).Include(s => s.Employees);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SectionsAssigneds/Details/5
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

        // GET: SectionsAssigneds/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Email", "Email");
            return View();
        }

        // POST: SectionsAssigneds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,CourseId,Section")] SectionsAssigned sectionsAssigned)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sectionsAssigned);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", sectionsAssigned.CourseId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Email", "Email", sectionsAssigned.EmployeeId);
            return View(sectionsAssigned);
        }

        // GET: SectionsAssigneds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SectionsAssigned == null)
            {
                return NotFound();
            }

            var sectionsAssigned = await _context.SectionsAssigned.FindAsync(id);
            if (sectionsAssigned == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", sectionsAssigned.CourseId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Email", "Email", sectionsAssigned.EmployeeId);
            return View(sectionsAssigned);
        }

        // POST: SectionsAssigneds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,CourseId,Section")] SectionsAssigned sectionsAssigned)
        {
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", sectionsAssigned.CourseId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Email", "Email", sectionsAssigned.EmployeeId);
            return View(sectionsAssigned);
        }

        // GET: SectionsAssigneds/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: SectionsAssigneds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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

        private bool SectionsAssignedExists(int id)
        {
          return (_context.SectionsAssigned?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

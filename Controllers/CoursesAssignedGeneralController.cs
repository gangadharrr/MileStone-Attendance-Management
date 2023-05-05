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
    public class CoursesAssignedGeneralController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesAssignedGeneralController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CoursesAssignedGeneral
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CoursesAssigned.Include(c => c.Courses).Include(c => c.Degrees);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CoursesAssignedGeneral/Details/5
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

        private bool CoursesAssignedExists(int id)
        {
          return (_context.CoursesAssigned?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

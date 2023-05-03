using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;
using MileStone_Attendance_Management.Data;
using MileStone_Attendance_Management.Models;
using OfficeOpenXml;
using Newtonsoft.Json;
using System.Formats.Asn1;
using NuGet.Versioning;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace MileStone_Attendance_Management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        
        public CoursesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
              return _context.Courses != null ? 
                          View(await _context.Courses.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Courses'  is null.");
        }


        // GET: Courses/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var courses = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (courses == null)
            {
                return NotFound();
            }

            return View(courses);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return _context.Courses != null ?
                          View(_context.Courses.ToList()) :
                          Problem("Entity set 'ApplicationDbContext.Courses'  is null.");
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,CourseType,CourseName")] Courses courses)
        {
            if (ModelState.IsValid)
            {
                try
                {   
                    _context.Add(courses);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            }
            return _context.Courses != null ?
                          View(_context.Courses.ToList()) :
                          Problem("Entity set 'ApplicationDbContext.Courses'  is null.");
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.Id = id;
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var courses = await _context.Courses.FindAsync(id);
            if (courses == null)
            {
                return NotFound();
            }
            return _context.Courses != null ?
                          View(await _context.Courses.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Courses'  is null.");
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CourseId,CourseType,CourseName")] Courses courses)
        {
            if (id != courses.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courses);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoursesExists(courses.CourseId))
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
            return _context.Courses != null ?
                          View(await _context.Courses.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Courses'  is null.");
        }

        
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.Courses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Courses'  is null.");
            }
            var courses = await _context.Courses.FindAsync(id);
            if (courses != null)
            {
                _context.Courses.Remove(courses);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult DeleteAll()
        {
            if (_context.Courses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Courses'  is null.");
            }

            try 
            {
                var courses = _context.Courses != null ?
                               _context.Courses.ToList() :
                              new List<Courses>();
                if (courses != null)
                {
                    foreach (var item in courses)
                    {
                        _context.Courses.Remove(item);
                        _context.SaveChanges();
                    }
                    return RedirectToAction(nameof(Index));
                }

            }
            catch(DBConcurrencyException ex)
            {
                return Problem(ex.Message);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            return RedirectToAction(nameof(Index));
            
        } 
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IFormFile UploadedFile)
        {
            if (Request != null)
            {
                if (Request.Form.Files.Count != 0 && !string.IsNullOrEmpty(UploadedFile.FileName))
                {

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files\", UploadedFile.FileName);

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
                    List<Courses> courseList=new List<Courses>();
                    bool _firstEntryLoop = true;
                    while (csvReader.Read())
                    {
                        if (_firstEntryLoop)
                        {
                            try 
                            { 
                                _firstEntryLoop = false;
                                var course = new Courses();
                                course.CourseId = csvReader.GetField<string>(0);
                                course.CourseType = csvReader.GetField<string>(1);
                                course.CourseName = csvReader.GetField<string>(2);
                                if (course.CourseId == "CourseId" && course.CourseType == "CourseType" && course.CourseName == "CourseName")
                                {
                                    continue;
                                }
                                else
                                {
                                    return Problem("Incorrect Format of Fields Try Again");
                                }
                            }
                            catch(Exception ex) 
                            {
                                Problem(ex.Message);
                            }
                        }
                        else 
                        { 
                            var course = new Courses();
                            course.CourseId = csvReader.GetField<string>(0);
                            course.CourseType = csvReader.GetField<string>(1);
                            course.CourseName = csvReader.GetField<string>(2);
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
                    catch(Exception e)
                    {
                        return Problem(e.Message);
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
                return RedirectToAction(nameof(Index), "Home");
        }
        private bool CoursesExists(string id)
        {
          return (_context.Courses?.Any(e => e.CourseId == id)).GetValueOrDefault();
        }
    }
}

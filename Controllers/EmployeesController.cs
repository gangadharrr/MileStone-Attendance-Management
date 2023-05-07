using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MileStone_Attendance_Management.Data;
using MileStone_Attendance_Management.Models;

namespace MileStone_Attendance_Management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public EmployeesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees.Include(e => e.Degrees).Include(e => e.Roles);
            return View(await applicationDbContext.ToListAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IFormFile UploadedFile)
        {
            if (Request != null)
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files\", UploadedFile.FileName);
                if (System.IO.File.Exists(filePath))
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
                    List<Employees> employeeList = new List<Employees>();
                    bool _firstEntryLoop = true;
                    while (csvReader.Read())
                    {
                        if (_firstEntryLoop)
                        {
                            try
                            {
                                _firstEntryLoop = false;
                                var employee = new Employees();
                                employee.EmployeeId = csvReader.GetField<string>(0);
                                employee.Name = csvReader.GetField<string>(1);
                                employee.Email = csvReader.GetField<string>(2);
                                employee.Designation = csvReader.GetField<string>(3);
                                employee.NormalizedDegree = csvReader.GetField<string>(4);
                                employee.NormalizedBranch = csvReader.GetField<string>(5);
                                if (employee.EmployeeId == "EmployeeId" && 
                                    employee.Name == "Name" &&
                                    employee.Email == "Email" &&
                                    employee.Designation == "Designation" &&
                                    employee.NormalizedDegree == "NormalizedDegree" &&
                                    employee.NormalizedBranch == "NormalizedBranch"
                                    )
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
                            catch (Exception ex)
                            {
                                return Problem(ex.Message);

                            }
                        }
                        else
                        {
                            var employee = new Employees();
                            employee.EmployeeId = csvReader.GetField<string>(0);
                            employee.Name = csvReader.GetField<string>(1);
                            employee.Email = csvReader.GetField<string>(2);
                            employee.Designation = csvReader.GetField<string>(3);
                            employee.Designation = _context.Roles.Where(m => m.Name == employee.Designation).Select(m => m.Id).FirstOrDefault();
                            employee.NormalizedDegree = csvReader.GetField<string>(4);
                            employee.NormalizedBranch = csvReader.GetField<string>(5);
                            employeeList.Add(employee);
                        }
                    }
                    try
                    {
                        foreach (var employee in employeeList)
                        {
                            _context.Add(employee);
                        }
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        streamReader.Close();
                        System.IO.File.Delete(filePath);
                        return Problem("Forien Key error Check the Degrees and Email" + e.Message);
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees
                .Include(e => e.Degrees)
                .Include(e => e.Roles)
                .FirstOrDefaultAsync(m => m.Email == id);
            if (employees == null)
            {
                return NotFound();
            }

            return View(employees);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
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
            ViewData["NormalizedDegree"] = _context.Degrees.ToList();
            ViewData["Designation"] = _context.Roles.ToList();
          
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            Employees employees = new Employees();
            employees.EmployeeId = collection["EmployeeId"].ToString();
            employees.Name = collection["Name"].ToString();
            employees.Email = collection["Email"].ToString();
            employees.Designation = collection["Designation"].ToString();
            employees.NormalizedDegree = collection["NormalizedDegree"].ToString();
            employees.NormalizedBranch = collection["NormalizedBranch"].ToString();
            Dictionary<string, List<string>> _branchesList = new Dictionary<string, List<string>>();
            foreach (var degree in _context.Degrees.ToList())
            {
                _branchesList.Add(degree.NormalizedDegree, _context.Branches.Where(m => m.NormalizedDegree == degree.NormalizedDegree).Select(m => m.NormalizedBranch).ToList());
      
            }
            ViewBag.BranchesList = _branchesList;
            if (ModelState.IsValid)
            { 
                try
                {
                    _context.Add(employees);
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex) 
                {
                    Problem(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["NormalizedDegree"] = _context.Degrees.ToList();
            ViewData["Designation"] = _context.Roles.ToList();
            return View(employees);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees.FindAsync(id);
            if (employees == null)
            {
                return NotFound();
            }
            Dictionary<string, List<string>> _branchesList = new Dictionary<string, List<string>>();
            foreach (var degree in _context.Degrees.ToList())
            {
                _branchesList.Add(degree.NormalizedDegree, _context.Branches.Where(m => m.NormalizedDegree == degree.NormalizedDegree).Select(m => m.NormalizedBranch).ToList());
            }
            ViewBag.BranchesList = _branchesList;
            ViewData["NormalizedDegree"] = _context.Degrees.ToList();
            ViewData["Designation"] = _context.Roles.ToList();
            return View(employees);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IFormCollection collection)
        {
            Employees employees = _context.Employees.Find(id);
            if (employees!=null && id != employees.Email)
            {
                return NotFound();
            }
            employees.EmployeeId = collection["EmployeeId"].ToString();
            employees.Name = collection["Name"].ToString();
            employees.Designation = collection["Designation"].ToString();
            employees.NormalizedDegree = collection["NormalizedDegree"].ToString();
            employees.NormalizedBranch = collection["NormalizedBranch"].ToString();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employees);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeesExists(employees.Email))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["NormalizedDegree"] = new SelectList(_context.Degrees, "NormalizedDegree", "NormalizedDegree", employees.NormalizedDegree);
            ViewData["Designation"] = new SelectList(_context.Roles, "Id", "Id", employees.Designation);
            return View(employees);
        }

      
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Employees'  is null.");
            }
            var employees = await _context.Employees.FindAsync(id);
            if (employees != null)
            {
                _context.Employees.Remove(employees);
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
            if (_context.Employees == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Employees'  is null.");
            }
            var employees = _context.Employees != null ?
                           _context.Employees.ToList() :
                          new List<Employees>();
            if (employees != null)
            {
                try
                {
                    foreach (var item in employees)
                    {
                        _context.Employees.Remove(item);
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
        private bool EmployeesExists(string id)
        {
          return (_context.Employees?.Any(e => e.Email == id)).GetValueOrDefault();
        }
    }
}

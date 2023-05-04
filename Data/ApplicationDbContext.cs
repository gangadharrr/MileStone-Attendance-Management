using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MileStone_Attendance_Management.Models;

namespace MileStone_Attendance_Management.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Degrees> Degrees { get; set; }
        public DbSet<Branches> Branches { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<CoursesAssigned> CoursesAssigned { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<SectionsAssigned> SectionsAssigned { get; set; }

        /*  protected override void OnModelCreating(ModelBuilder builder)
          {
              *//*builder.Entity<Students>()
                  .HasIndex(u => u.RollNumber)
                  .IsUnique();*//*
              builder.Entity<Degrees>()
                  .HasIndex(u => u.Degree)
                  .IsUnique();

          }*/
    }
}
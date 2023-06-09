﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MileStone_Attendance_Management.Models
{
    public class SectionsAssigned
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Employees")]
        [Display(Name = "Employee Email")]
        [Required(ErrorMessage = "Employee Email is Required")]
        public string Email { get; set; }
        [ForeignKey("Courses")]
        [Display(Name = "Course Id")]
        [Required(ErrorMessage = "Course Id is Required")]
        public string CourseId { get; set; }
        [Display(Name = "Section")]
        [Required(ErrorMessage = "Section is Required")]
        public string Section { get; set; }
        public string NormalizedDegree { get; set; } = " ";
        public string NormalizedBranch { get; set; } = " ";
        public virtual Employees? Employees { get; set; }
        public virtual Courses? Courses { get; set; }
    }
}

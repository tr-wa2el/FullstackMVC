namespace FullstackMVC.Models
{
    using FullstackMVC.Attributes;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Index(nameof(Email), IsUnique = true)]
    [InstructorDepartmentSalary]
    public class Instructor
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Address { get; set; }

        public int Age { get; set; }

        public decimal Salary { get; set; }

        public string? Degree { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [ForeignKey(nameof(Department))]
        public int? DeptId { get; set; }

        public Department? Department { get; set; }

        public ICollection<Course>? Courses { get; set; }
    }
}

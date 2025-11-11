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

        [Required(ErrorMessage = "Instructor name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [Display(Name = "Instructor Name")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Range(25, 100, ErrorMessage = "Age must be between 25 and 100")]
        [Display(Name = "Age")]
        public int Age { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        [Display(Name = "Salary")]
        public decimal Salary { get; set; }

        [StringLength(100, ErrorMessage = "Degree cannot exceed 100 characters")]
        [Display(Name = "Degree")]
        public string? Degree { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [Display(Name = "Profile Image URL")]
        public string? Image { get; set; }

        [ForeignKey(nameof(Department))]
        [Display(Name = "Department")]
        public int? DeptId { get; set; }

        public Department? Department { get; set; }

        public ICollection<Course>? Courses { get; set; }
    }
}

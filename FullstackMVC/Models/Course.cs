namespace FullstackMVC.Models
{
    using FullstackMVC.Attributes;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Course
    {
        [Key]
        public int Num { get; set; }

        [Required(ErrorMessage = "Course name is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Course name must be between 2 and 20 characters")]
        [UniqueCourseNameAttribute]
        [Display(Name = "Course Name")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "Topic cannot exceed 50 characters")]
        [Display(Name = "Course Topic")]
        public string? Topic { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Course Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Maximum degree is required")]
        [Range(100, 120, ErrorMessage = "Degree must be between 100 and 120")]
        [Display(Name = "Maximum Degree")]
        public decimal Degree { get; set; }

        [Required(ErrorMessage = "Minimum degree is required")]
        [Range(50, 60, ErrorMessage = "Minimum degree must be between 50 and 60")]
        [Display(Name = "Minimum Degree")]
        public decimal MinDegree { get; set; }

        [ForeignKey(nameof(Department))]
        [Required(ErrorMessage = "Department is required")]
        public int DeptId { get; set; }

        public Department? Department { get; set; }

        // Primary instructor for the course
        [ForeignKey(nameof(Instructor))]
        [Display(Name = "Instructor")]
        public int? InstructorId { get; set; }

        public Instructor? Instructor { get; set; }

        public ICollection<Grade>? Grades { get; set; }

        public ICollection<Instructor>? Instructors { get; set; }
    }
}

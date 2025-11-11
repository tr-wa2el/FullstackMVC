namespace FullstackMVC.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters")]
        [Display(Name = "Department Name")]
        public string? Name { get; set; }

        [Display(Name = "PC Numbers")]
        public int? PcNumbers { get; set; }

        [StringLength(100, ErrorMessage = "Manager name cannot exceed 100 characters")]
        [Display(Name = "Manager Name")]
        public string? ManagerName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
        [Display(Name = "Location")]
        public string? Location { get; set; }

        public ICollection<Employee>? Employees { get; set; }

        public ICollection<Student>? Students { get; set; }

        public ICollection<Instructor>? Instructors { get; set; }

        public ICollection<Course>? Courses { get; set; }
    }
}

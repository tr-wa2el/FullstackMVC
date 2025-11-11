namespace FullstackMVC.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "SSN is required")]
        [MinLength(4, ErrorMessage = "SSN must be at least 4 digits")]
        [Display(Name = "SSN")]
        public int SSN { get; set; }

        [Required(ErrorMessage = "Student name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
        [Display(Name = "Student Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(16, 100, ErrorMessage = "Age must be between 16 and 100")]
        [Display(Name = "Age")]
        public int Age { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [Display(Name = "Profile Image URL")]
        public string? Image { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either 'Male' or 'Female'")]
        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [ForeignKey(nameof(Department))]
        [Display(Name = "Department")]
        public int? DeptId { get; set; }

        public Department? Department { get; set; }

        public ICollection<Grade>? Grades { get; set; }
    }
}

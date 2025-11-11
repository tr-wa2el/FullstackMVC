namespace FullstackMVC.Models
{
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Employee
    {
        [Key]
        public int SSN { get; set; }

        public string Name { get; set; }

        public string? Address { get; set; }

        public string? Image { get; set; }

        public int Salary { get; set; }

        [ForeignKey(nameof(Department))]
        public int? DeptId { get; set; }

        public Department? Department { get; set; }
    }
}

namespace FullstackMVC.Models
{
    public class Department
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? PcNumbers { get; set; }

        public string? MangerName { get; set; }

        public string? Location { get; set; }

        public ICollection<Employee>? Employees { get; set; }

        public ICollection<Student>? Students { get; set; }

        public ICollection<Instructor>? Instructors { get; set; }

        public ICollection<Course>? Courses { get; set; }
    }
}

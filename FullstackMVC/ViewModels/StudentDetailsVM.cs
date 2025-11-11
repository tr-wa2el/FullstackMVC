namespace FullstackMVC.ViewModels
{
    public class StudentDetailsVM
    {
        public int SSN { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string? Gender { get; set; }

        public string? Address { get; set; }

        public string? Image { get; set; }

        public string DepartmentName { get; set; }

        public string Message { get; set; }

        public int GradesCount { get; set; }

        public decimal? AverageGrade { get; set; }

        public string PerformanceColor { get; set; }

        public List<CourseGradeVM> CourseGrades { get; set; } = new List<CourseGradeVM>();
    }

    public class CourseGradeVM
    {
        public string CourseName { get; set; }

        public decimal? GradeValue { get; set; }

        public string GradeStatus { get; set; }

        public string GradeColor { get; set; }
    }
}

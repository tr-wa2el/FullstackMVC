namespace FullstackMVC.ViewModels
{
    public class StudentCourseVM
    {
        public int StudentSSN { get; set; }

        public string StudentName { get; set; }

        public int StudentAge { get; set; }

        public string? StudentGender { get; set; }

        public string DepartmentName { get; set; }

        public string Message { get; set; }

        public int TotalCoursesEnrolled { get; set; }

        public int PassedCoursesCount { get; set; }

        public int FailedCoursesCount { get; set; }

        public decimal? OverallAverage { get; set; }

        public string PerformanceLevel { get; set; }

        public string PerformanceColor { get; set; }

        public List<CourseEnrollmentVM> Courses { get; set; } = new List<CourseEnrollmentVM>();
    }

    public class CourseEnrollmentVM
    {
        public int CourseNum { get; set; }

        public string CourseName { get; set; }

        public string? CourseTopic { get; set; }

        public string? CourseDescription { get; set; }

        public decimal MaxDegree { get; set; }

        public decimal MinDegree { get; set; }

        public decimal? StudentGrade { get; set; }

        public string GradeStatus { get; set; }

        public string GradeColor { get; set; }

        public decimal? PercentageScore { get; set; }

        public bool IsAboveMinimum { get; set; }
    }
}

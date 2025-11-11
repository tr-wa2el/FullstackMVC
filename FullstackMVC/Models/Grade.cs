namespace FullstackMVC.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class Grade
    {
        public int Id { get; set; }

        public decimal? GradeValue { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentSSN { get; set; }

        public Student? Student { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseNum { get; set; }

        public Course? Course { get; set; }
    }
}

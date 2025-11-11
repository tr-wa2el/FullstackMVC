namespace FullstackMVC.Context
{
    using FullstackMVC.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class CompanyContext : IdentityDbContext<ApplicationUser>
    {
        public CompanyContext()
        {
        }

        public CompanyContext(DbContextOptions<CompanyContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Course-Department relationship
            modelBuilder
                .Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DeptId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Course-Instructor relationship (primary instructor)
            modelBuilder
                .Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed Departments
            modelBuilder
                .Entity<Department>()
                .HasData(
                    new Department
                    {
                        Id = 1,
                        Name = "Computer Science",
                        MangerName = "Dr. Ahmed Ali",
                        Location = "Building A",
                        PcNumbers = 50,
                    },
                    new Department
                    {
                        Id = 2,
                        Name = "Engineering",
                        MangerName = "Dr. Sara Hassan",
                        Location = "Building B",
                        PcNumbers = 40,
                    },
                    new Department
                    {
                        Id = 3,
                        Name = "Business Administration",
                        MangerName = "Dr. Mohamed Farid",
                        Location = "Building C",
                        PcNumbers = 30,
                    }
                );

            // Seed Courses - Now with DeptId (Names shortened to 20 chars max)
            modelBuilder
                .Entity<Course>()
                .HasData(
                    new Course
                    {
                        Num = 1,
                        Name = "Intro Programming",
                        Topic = "Programming",
                        Description = "Learn basics of programming",
                        Degree = 100,
                        MinDegree = 50,
                        DeptId = 1,
                    },
                    new Course
                    {
                        Num = 2,
                        Name = "Data Structures",
                        Topic = "Computer Science",
                        Description = "Study of data organization",
                        Degree = 100,
                        MinDegree = 50,
                        DeptId = 1,
                    },
                    new Course
                    {
                        Num = 3,
                        Name = "Database Systems",
                        Topic = "Databases",
                        Description = "Relational databases and SQL",
                        Degree = 100,
                        MinDegree = 50,
                        DeptId = 1,
                    },
                    new Course
                    {
                        Num = 4,
                        Name = "Web Development",
                        Topic = "Web",
                        Description = "HTML, CSS, JavaScript, ASP.NET",
                        Degree = 100,
                        MinDegree = 50,
                        DeptId = 1,
                    },
                    new Course
                    {
                        Num = 5,
                        Name = "Machine Learning",
                        Topic = "AI",
                        Description = "Introduction to ML algorithms",
                        Degree = 100,
                        MinDegree = 50,
                        DeptId = 1,
                    }
                );

            // Seed Instructors - Now with Email
            modelBuilder
                .Entity<Instructor>()
                .HasData(
                    new Instructor
                    {
                        Id = 1,
                        Name = "Dr. Amr Khaled",
                        Address = "Cairo, Egypt",
                        Age = 45,
                        Salary = 8000,
                        Degree = "PhD",
                        Email = "amr.khaled@university.edu",
                        DeptId = 1,
                    },
                    new Instructor
                    {
                        Id = 2,
                        Name = "Dr. Mona Sayed",
                        Address = "Alexandria, Egypt",
                        Age = 40,
                        Salary = 7500,
                        Degree = "PhD",
                        Email = "mona.sayed@university.edu",
                        DeptId = 1,
                    },
                    new Instructor
                    {
                        Id = 3,
                        Name = "Dr. Yasser Ibrahim",
                        Address = "Giza, Egypt",
                        Age = 38,
                        Salary = 7000,
                        Degree = "PhD",
                        Email = "yasser.ibrahim@university.edu",
                        DeptId = 2,
                    },
                    new Instructor
                    {
                        Id = 4,
                        Name = "Dr. Hanan Mahmoud",
                        Address = "Mansoura, Egypt",
                        Age = 42,
                        Salary = 7200,
                        Degree = "PhD",
                        Email = "hanan.mahmoud@university.edu",
                        DeptId = 3,
                    }
                );

            // Seed Students
            modelBuilder
                .Entity<Student>()
                .HasData(
                    new Student
                    {
                        SSN = 1001,
                        Name = "Ahmed Mohamed",
                        Age = 20,
                        Address = "Cairo, Egypt",
                        Gender = "Male",
                        DeptId = 1,
                    },
                    new Student
                    {
                        SSN = 1002,
                        Name = "Fatma Hassan",
                        Age = 21,
                        Address = "Alexandria, Egypt",
                        Gender = "Female",
                        DeptId = 1,
                    },
                    new Student
                    {
                        SSN = 1003,
                        Name = "Omar Khaled",
                        Age = 22,
                        Address = "Giza, Egypt",
                        Gender = "Male",
                        DeptId = 2,
                    },
                    new Student
                    {
                        SSN = 1004,
                        Name = "Nour Elsayed",
                        Age = 20,
                        Address = "Mansoura, Egypt",
                        Gender = "Female",
                        DeptId = 1,
                    },
                    new Student
                    {
                        SSN = 1005,
                        Name = "Karim Ali",
                        Age = 23,
                        Address = "Tanta, Egypt",
                        Gender = "Male",
                        DeptId = 2,
                    },
                    new Student
                    {
                        SSN = 1006,
                        Name = "Mariam Youssef",
                        Age = 21,
                        Address = "Aswan, Egypt",
                        Gender = "Female",
                        DeptId = 3,
                    },
                    new Student
                    {
                        SSN = 1007,
                        Name = "Youssef Mahmoud",
                        Age = 22,
                        Address = "Luxor, Egypt",
                        Gender = "Male",
                        DeptId = 3,
                    },
                    new Student
                    {
                        SSN = 1008,
                        Name = "Heba Ahmed",
                        Age = 20,
                        Address = "Port Said, Egypt",
                        Gender = "Female",
                        DeptId = 1,
                    }
                );

            // Seed Grades
            modelBuilder
                .Entity<Grade>()
                .HasData(
                    // Ahmed Mohamed's grades
                    new Grade
                    {
                        Id = 1,
                        StudentSSN = 1001,
                        CourseNum = 1,
                        GradeValue = 85,
                    },
                    new Grade
                    {
                        Id = 2,
                        StudentSSN = 1001,
                        CourseNum = 2,
                        GradeValue = 90,
                    },
                    new Grade
                    {
                        Id = 3,
                        StudentSSN = 1001,
                        CourseNum = 3,
                        GradeValue = 88,
                    },
                    // Fatma Hassan's grades
                    new Grade
                    {
                        Id = 4,
                        StudentSSN = 1002,
                        CourseNum = 1,
                        GradeValue = 92,
                    },
                    new Grade
                    {
                        Id = 5,
                        StudentSSN = 1002,
                        CourseNum = 2,
                        GradeValue = 95,
                    },
                    new Grade
                    {
                        Id = 6,
                        StudentSSN = 1002,
                        CourseNum = 4,
                        GradeValue = 89,
                    },
                    // Omar Khaled's grades
                    new Grade
                    {
                        Id = 7,
                        StudentSSN = 1003,
                        CourseNum = 2,
                        GradeValue = 78,
                    },
                    new Grade
                    {
                        Id = 8,
                        StudentSSN = 1003,
                        CourseNum = 3,
                        GradeValue = 82,
                    },
                    // Nour Elsayed's grades
                    new Grade
                    {
                        Id = 9,
                        StudentSSN = 1004,
                        CourseNum = 1,
                        GradeValue = 88,
                    },
                    new Grade
                    {
                        Id = 10,
                        StudentSSN = 1004,
                        CourseNum = 3,
                        GradeValue = 91,
                    },
                    new Grade
                    {
                        Id = 11,
                        StudentSSN = 1004,
                        CourseNum = 4,
                        GradeValue = 87,
                    },
                    // Karim Ali's grades
                    new Grade
                    {
                        Id = 12,
                        StudentSSN = 1005,
                        CourseNum = 2,
                        GradeValue = 75,
                    },
                    new Grade
                    {
                        Id = 13,
                        StudentSSN = 1005,
                        CourseNum = 5,
                        GradeValue = 80,
                    },
                    // Mariam Youssef's grades
                    new Grade
                    {
                        Id = 14,
                        StudentSSN = 1006,
                        CourseNum = 1,
                        GradeValue = 93,
                    },
                    new Grade
                    {
                        Id = 15,
                        StudentSSN = 1006,
                        CourseNum = 4,
                        GradeValue = 90,
                    },
                    // Youssef Mahmoud's grades
                    new Grade
                    {
                        Id = 16,
                        StudentSSN = 1007,
                        CourseNum = 3,
                        GradeValue = 84,
                    },
                    new Grade
                    {
                        Id = 17,
                        StudentSSN = 1007,
                        CourseNum = 4,
                        GradeValue = 86,
                    },
                    // Heba Ahmed's grades
                    new Grade
                    {
                        Id = 18,
                        StudentSSN = 1008,
                        CourseNum = 1,
                        GradeValue = 96,
                    },
                    new Grade
                    {
                        Id = 19,
                        StudentSSN = 1008,
                        CourseNum = 2,
                        GradeValue = 94,
                    },
                    new Grade
                    {
                        Id = 20,
                        StudentSSN = 1008,
                        CourseNum = 5,
                        GradeValue = 92,
                    }
                );
        }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Instructor> Instructors { get; set; }

        public DbSet<Grade> Grades { get; set; }
    }
}

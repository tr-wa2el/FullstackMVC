namespace FullstackMVC.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    internal partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PcNumbers = table.Column<int>(type: "int", nullable: true),
                    MangerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Num = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Degree = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinDegree = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeptId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Num);
                    table.ForeignKey(
                        name: "FK_Courses_Departments_DeptId",
                        column: x => x.DeptId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    SSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salary = table.Column<int>(type: "int", nullable: false),
                    DeptId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.SSN);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DeptId",
                        column: x => x.DeptId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeptId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instructors_Departments_DeptId",
                        column: x => x.DeptId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    SSN = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeptId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.SSN);
                    table.ForeignKey(
                        name: "FK_Students_Departments_DeptId",
                        column: x => x.DeptId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseInstructor",
                columns: table => new
                {
                    CoursesNum = table.Column<int>(type: "int", nullable: false),
                    InstructorsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseInstructor", x => new { x.CoursesNum, x.InstructorsId });
                    table.ForeignKey(
                        name: "FK_CourseInstructor_Courses_CoursesNum",
                        column: x => x.CoursesNum,
                        principalTable: "Courses",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseInstructor_Instructors_InstructorsId",
                        column: x => x.InstructorsId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StudentSSN = table.Column<int>(type: "int", nullable: false),
                    CourseNum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grades_Courses_CourseNum",
                        column: x => x.CourseNum,
                        principalTable: "Courses",
                        principalColumn: "Num",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grades_Students_StudentSSN",
                        column: x => x.StudentSSN,
                        principalTable: "Students",
                        principalColumn: "SSN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Location", "MangerName", "Name", "PcNumbers" },
                values: new object[,]
                {
                    { 1, "Building A", "Dr. Ahmed Ali", "Computer Science", 50 },
                    { 2, "Building B", "Dr. Sara Hassan", "Engineering", 40 },
                    { 3, "Building C", "Dr. Mohamed Farid", "Business Administration", 30 }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Num", "Degree", "DeptId", "Description", "MinDegree", "Name", "Topic" },
                values: new object[,]
                {
                    { 1, 100m, 1, "Learn basics of programming", 50m, "Intro Programming", "Programming" },
                    { 2, 100m, 1, "Study of data organization", 50m, "Data Structures", "Computer Science" },
                    { 3, 100m, 1, "Relational databases and SQL", 50m, "Database Systems", "Databases" },
                    { 4, 100m, 1, "HTML, CSS, JavaScript, ASP.NET", 50m, "Web Development", "Web" },
                    { 5, 100m, 1, "Introduction to ML algorithms", 50m, "Machine Learning", "AI" }
                });

            migrationBuilder.InsertData(
                table: "Instructors",
                columns: new[] { "Id", "Address", "Age", "Degree", "DeptId", "Email", "Name", "Salary" },
                values: new object[,]
                {
                    { 1, "Cairo, Egypt", 45, "PhD", 1, "amr.khaled@university.edu", "Dr. Amr Khaled", 8000m },
                    { 2, "Alexandria, Egypt", 40, "PhD", 1, "mona.sayed@university.edu", "Dr. Mona Sayed", 7500m },
                    { 3, "Giza, Egypt", 38, "PhD", 2, "yasser.ibrahim@university.edu", "Dr. Yasser Ibrahim", 7000m },
                    { 4, "Mansoura, Egypt", 42, "PhD", 3, "hanan.mahmoud@university.edu", "Dr. Hanan Mahmoud", 7200m }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "SSN", "Address", "Age", "DeptId", "Gender", "Image", "Name" },
                values: new object[,]
                {
                    { 1001, "Cairo, Egypt", 20, 1, "Male", null, "Ahmed Mohamed" },
                    { 1002, "Alexandria, Egypt", 21, 1, "Female", null, "Fatma Hassan" },
                    { 1003, "Giza, Egypt", 22, 2, "Male", null, "Omar Khaled" },
                    { 1004, "Mansoura, Egypt", 20, 1, "Female", null, "Nour Elsayed" },
                    { 1005, "Tanta, Egypt", 23, 2, "Male", null, "Karim Ali" },
                    { 1006, "Aswan, Egypt", 21, 3, "Female", null, "Mariam Youssef" },
                    { 1007, "Luxor, Egypt", 22, 3, "Male", null, "Youssef Mahmoud" },
                    { 1008, "Port Said, Egypt", 20, 1, "Female", null, "Heba Ahmed" }
                });

            migrationBuilder.InsertData(
                table: "Grades",
                columns: new[] { "Id", "CourseNum", "GradeValue", "StudentSSN" },
                values: new object[,]
                {
                    { 1, 1, 85m, 1001 },
                    { 2, 2, 90m, 1001 },
                    { 3, 3, 88m, 1001 },
                    { 4, 1, 92m, 1002 },
                    { 5, 2, 95m, 1002 },
                    { 6, 4, 89m, 1002 },
                    { 7, 2, 78m, 1003 },
                    { 8, 3, 82m, 1003 },
                    { 9, 1, 88m, 1004 },
                    { 10, 3, 91m, 1004 },
                    { 11, 4, 87m, 1004 },
                    { 12, 2, 75m, 1005 },
                    { 13, 5, 80m, 1005 },
                    { 14, 1, 93m, 1006 },
                    { 15, 4, 90m, 1006 },
                    { 16, 3, 84m, 1007 },
                    { 17, 4, 86m, 1007 },
                    { 18, 1, 96m, 1008 },
                    { 19, 2, 94m, 1008 },
                    { 20, 5, 92m, 1008 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseInstructor_InstructorsId",
                table: "CourseInstructor",
                column: "InstructorsId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DeptId",
                table: "Courses",
                column: "DeptId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DeptId",
                table: "Employees",
                column: "DeptId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_CourseNum",
                table: "Grades",
                column: "CourseNum");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_StudentSSN",
                table: "Grades",
                column: "StudentSSN");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_DeptId",
                table: "Instructors",
                column: "DeptId");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_Email",
                table: "Instructors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_DeptId",
                table: "Students",
                column: "DeptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseInstructor");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Instructors");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FullstackMVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDepartmentSchemaFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Description column to Departments
            migrationBuilder.AddColumn<string>(
            name: "Description",
      table: "Departments",
    type: "nvarchar(500)",
       maxLength: 500,
       nullable: true);

         // Rename MangerName to ManagerName if the old column exists
         migrationBuilder.Sql(@"
      IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND name = 'MangerName')
  AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND name = 'ManagerName')
         BEGIN
      EXEC sp_rename 'Departments.MangerName', 'ManagerName', 'COLUMN'
         END
          ");

       // Add Image column to Instructors
            migrationBuilder.AddColumn<string>(
      name: "Image",
      table: "Instructors",
              type: "nvarchar(500)",
    maxLength: 500,
           nullable: true);

      // Update seed data to use new column names
         migrationBuilder.UpdateData(
        table: "Departments",
              keyColumn: "Id",
                keyValue: 1,
        columns: new[] { "Description", "ManagerName" },
     values: new object[] { "Department of Computer Science and Information Technology", "Dr. Ahmed Ali" });

      migrationBuilder.UpdateData(
         table: "Departments",
         keyColumn: "Id",
   keyValue: 2,
       columns: new[] { "Description", "ManagerName" },
      values: new object[] { "Department of Engineering and Applied Sciences", "Dr. Sara Hassan" });

       migrationBuilder.UpdateData(
  table: "Departments",
                keyColumn: "Id",
 keyValue: 3,
       columns: new[] { "Description", "ManagerName" },
    values: new object[] { "Department of Business Administration and Management", "Dr. Mohamed Farid" });
 }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
     // Remove Description column from Departments
    migrationBuilder.DropColumn(
       name: "Description",
      table: "Departments");

   // Remove Image column from Instructors
            migrationBuilder.DropColumn(
          name: "Image",
    table: "Instructors");

    // Rename back to MangerName
   migrationBuilder.Sql(@"
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Departments]') AND name = 'ManagerName')
                BEGIN
     EXEC sp_rename 'Departments.ManagerName', 'MangerName', 'COLUMN'
        END
       ");
        }
    }
}
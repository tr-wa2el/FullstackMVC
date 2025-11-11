namespace FullstackMVC.Attributes
{
    using FullstackMVC.Context;
    using System.ComponentModel.DataAnnotations;

    public class UniqueCourseNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var courseName = value.ToString();
            var courseInstance = validationContext.ObjectInstance as dynamic;

            // Get DeptId from the model
            int? deptId = courseInstance?.DeptId;
            int? courseNum = courseInstance?.Num;

            if (!deptId.HasValue)
            {
                return ValidationResult.Success;
            }

            using (var context = new CompanyContext())
            {
                // Check if course name exists in the same department
                var exists = context.Courses.Any(c =>
                    c.Name == courseName && c.DeptId == deptId && c.Num != courseNum
                );

                if (exists)
                {
                    return new ValidationResult(
                        $"A course with the name '{courseName}' already exists in this department."
                    );
                }
            }

            return ValidationResult.Success;
        }
    }
}

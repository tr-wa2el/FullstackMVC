namespace FullstackMVC.Attributes
{
    using FullstackMVC.Context;
    using System.ComponentModel.DataAnnotations;

    public class InstructorDepartmentSalaryAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var instructorInstance = validationContext.ObjectInstance as dynamic;

            if (instructorInstance == null)
            {
                return ValidationResult.Success;
            }

            int? deptId = instructorInstance.DeptId;
            decimal salary = instructorInstance.Salary;

            if (!deptId.HasValue)
            {
                return ValidationResult.Success;
            }

            using (var context = new CompanyContext())
            {
                var department = context.Departments.Find(deptId.Value);

                if (department == null)
                {
                    return ValidationResult.Success;
                }

                // SD department (assuming SD or Software Development) - salary must be > 10000
                if (
                    department.Name?.ToLower().Contains("sd") == true
                    || department.Name?.ToLower().Contains("software development") == true
                )
                {
                    if (salary <= 10000)
                    {
                        return new ValidationResult(
                            $"For {department.Name} department, salary must be greater than 10,000."
                        );
                    }
                }
                // BC/Business/Commerce department - salary must be > 15000 AND < 50000
                else if (
                    department.Name?.ToLower().Contains("hr") == true
                    || department.Name?.ToLower().Contains("business") == true
                    || department.Name?.ToLower().Contains("commerce") == true
                )
                {
                    if (salary <= 15000 || salary >= 50000)
                    {
                        return new ValidationResult(
                            $"For {department.Name} department, salary must be greater than 15,000 and less than 50,000."
                        );
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}

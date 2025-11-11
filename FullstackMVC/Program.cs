namespace FullstackMVC
{
    using FullstackMVC.Context;
    using FullstackMVC.Middleware;
    using FullstackMVC.Models;
    using FullstackMVC.Repositories.Implementations;
    using FullstackMVC.Repositories.Interfaces;
    using FullstackMVC.Services.Implementations;
    using FullstackMVC.Services.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                // Optional: Register filters globally for all controllers
                // Uncomment the following lines to apply filters globally:

                // options.Filters.Add<GlobalExceptionFilter>();
                // options.Filters.Add<ResourceOptimizationFilter>();
                // options.Filters.Add<DepartmentLocationActionFilter>();
            });

            builder.Services.AddDbContext<CompanyContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // Configure Identity
            builder
                .Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings
                    options.User.RequireUniqueEmail = true;

                    // Sign in settings - Enable email confirmation
                    options.SignIn.RequireConfirmedEmail = false; // Set to true in production
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                })
                .AddEntityFrameworkStores<CompanyContext>()
                .AddDefaultTokenProviders();

            // Configure Facebook Authentication
            builder
                .Services.AddAuthentication()
                .AddFacebook(options =>
                {
                    options.AppId = builder.Configuration["Authentication:Facebook:AppId"] ?? "";
                    options.AppSecret =
                        builder.Configuration["Authentication:Facebook:AppSecret"] ?? "";
                    options.SaveTokens = true;
                });

            // Configure application cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            // Register HttpClient for WhatsApp service
            builder.Services.AddHttpClient();

            // Register custom services
            builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Dependency Inversion: Controllers
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IInstructorService, InstructorService>();

            var app = builder.Build();

            // Seed roles and test accounts
            await SeedRolesAndTestAccountsAsync(app.Services);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Custom Middleware Pipeline
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseMiddleware<RateLimitingMiddleware>(100, 60); // 100 requests per 60 seconds

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                )
                .WithStaticAssets();

            app.Run();
        }

        private static async Task SeedRolesAndTestAccountsAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<
                UserManager<ApplicationUser>
            >();
            var context = scope.ServiceProvider.GetRequiredService<CompanyContext>();

            // Seed roles
            string[] roleNames = { "Admin", "Student", "Instructor" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Admin user
            var adminEmail = "admin@university.edu";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    IsPhoneVerified = true,
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Seed Student test account (linked to Ahmed Mohamed - SSN 1001)
            var studentEmail = "student@university.edu";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);

            if (studentUser == null)
            {
                var student = await context.Students.FirstOrDefaultAsync(s => s.SSN == 1001);
                if (student != null)
                {
                    var studentAccount = new ApplicationUser
                    {
                        UserName = "student1001",
                        Email = studentEmail,
                        FullName = student.Name,
                        EmailConfirmed = true,
                        PhoneNumber = "1001234567",
                        PhoneNumberConfirmed = true,
                        IsPhoneVerified = true,
                        StudentSSN = student.SSN,
                        Address = student.Address,
                    };

                    var result = await userManager.CreateAsync(studentAccount, "Student@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(studentAccount, "Student");
                    }
                }
            }

            // Seed Instructor test account (linked to Dr. Amr Khaled - ID 1)
            var instructorEmail = "instructor@university.edu";
            var instructorUser = await userManager.FindByEmailAsync(instructorEmail);

            if (instructorUser == null)
            {
                var instructor = await context.Instructors.FirstOrDefaultAsync(i => i.Id == 1);
                if (instructor != null)
                {
                    var instructorAccount = new ApplicationUser
                    {
                        UserName = "instructor1",
                        Email = instructorEmail,
                        FullName = instructor.Name,
                        EmailConfirmed = true,
                        PhoneNumber = "1111234567",
                        PhoneNumberConfirmed = true,
                        IsPhoneVerified = true,
                        InstructorId = instructor.Id,
                        Address = instructor.Address,
                    };

                    var result = await userManager.CreateAsync(instructorAccount, "Instructor@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(instructorAccount, "Instructor");
                    }
                }
            }
        }
    }
}

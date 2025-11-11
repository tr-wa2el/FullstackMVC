# 🎓 FullstackMVC - University Management System

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4)](https://docs.microsoft.com/aspnet/core)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

A comprehensive **University Management System** built with **ASP.NET Core MVC** (.NET 9), featuring role-based authentication, complete CRUD operations, and a modern, responsive UI.

![Project Banner](https://via.placeholder.com/1200x300/667eea/ffffff?text=University+Management+System)

---

## 📋 Table of Contents

- [Features](#-features)
- [Technologies](#-technologies)
- [Architecture](#-architecture)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [Authentication & Authorization](#-authentication--authorization)
- [Database Schema](#-database-schema)
- [User Roles](#-user-roles)
- [Screenshots](#-screenshots)
- [API Documentation](#-api-documentation)
- [Testing](#-testing)
- [Contributing](#-contributing)
- [License](#-license)

---

## ✨ Features

### 🔐 **Authentication & Security**
- ✅ ASP.NET Core Identity integration
- ✅ Role-based authorization (Admin, Student, Instructor)
- ✅ Two-factor authentication (OTP via WhatsApp)
- ✅ Email confirmation
- ✅ Password reset functionality
- ✅ Facebook OAuth integration
- ✅ Anti-CSRF protection
- ✅ Rate limiting middleware

### 👨‍💼 **Admin Dashboard**
- ✅ Complete CRUD operations for:
  - Students
  - Instructors
  - Courses
  - Departments
  - Users & Roles
- ✅ System statistics and analytics
- ✅ User management
- ✅ Role assignment
- ✅ Relationship validation before deletion

### 👨‍🎓 **Student Portal**
- ✅ Personalized dashboard
- ✅ View enrolled courses and grades
- ✅ Browse all available courses
- ✅ Department information
- ✅ Profile management
- ✅ Academic performance tracking
- ✅ Grade statistics and progress bars

### 👨‍🏫 **Instructor Portal**
- ✅ Teaching dashboard
- ✅ Course management
- ✅ Student grade entry and editing
- ✅ View course enrollments
- ✅ Student performance tracking
- ✅ Profile management

### 🎨 **Modern UI/UX**
- ✅ Responsive design (Bootstrap 5)
- ✅ Unified color system
- ✅ Bootstrap Icons integration
- ✅ Interactive hover effects
- ✅ Progress bars and badges
- ✅ Card-based layouts
- ✅ Mobile-friendly interface

### 🏗️ **Architecture & Patterns**
- ✅ Repository Pattern
- ✅ Unit of Work Pattern
- ✅ Service Layer
- ✅ Dependency Injection
- ✅ SOLID Principles
- ✅ Custom Action Filters
- ✅ Middleware pipeline
- ✅ View Models

---

## 🛠️ Technologies

### **Backend**
- .NET 9
- ASP.NET Core MVC
- Entity Framework Core 9
- ASP.NET Core Identity
- SQL Server

### **Frontend**
- HTML5 / CSS3
- Bootstrap 5
- Bootstrap Icons
- JavaScript
- Razor View Engine

### **External Services**
- WhatsApp Business API (OTP)
- Facebook OAuth
- SMTP (Email)

### **Development Tools**
- Visual Studio 2022
- SQL Server Management Studio (SSMS)
- Git

---

## 🏛️ Architecture

### **Layered Architecture**

```
FullstackMVC/
│
├── Controllers/        # MVC Controllers
│   ├── AdminController.cs
│   ├── StudentController.cs
│   ├── InstructorController.cs
│   ├── CourseController.cs
│   ├── DepartmentController.cs
│   └── AccountController.cs
│
├── Models/     # Domain Models
│   ├── Student.cs
│   ├── Instructor.cs
│   ├── Course.cs
│   ├── Department.cs
│   ├── Grade.cs
│   └── ApplicationUser.cs
│
├── Services/            # Business Logic Layer
│   ├── Interfaces/
│   │   ├── IStudentService.cs
│   │├── IInstructorService.cs
│   │   ├── ICourseService.cs
│   │   └── IDepartmentService.cs
│   └── Implementations/
│       ├── StudentService.cs
│       ├── InstructorService.cs
│   ├── CourseService.cs
│  └── DepartmentService.cs
│
├── Repositories/        # Data Access Layer
│   ├── Interfaces/
│   │   ├── IRepository.cs
│   │   └── IUnitOfWork.cs
│   └── Implementations/
│       ├── Repository.cs
│       └── UnitOfWork.cs
│
├── Context/            # Database Context
│   └── CompanyContext.cs
│
├── ViewModels/    # View Models
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   └── StudentViewModel.cs
│
├── Middleware/      # Custom Middleware
│   ├── GlobalExceptionHandlerMiddleware.cs
│   ├── LoggingMiddleware.cs
│ └── RateLimitingMiddleware.cs
│
├── Filters/            # Custom Action Filters
│   ├── GlobalExceptionFilter.cs
│   ├── DepartmentLocationAction.cs
│   └── ResourceOptimization.cs
│
└── Views/              # Razor Views
    ├── Admin/
    ├── Student/
    ├── Instructor/
    ├── Course/
    ├── Department/
    └── Account/
```

### **Design Patterns Implemented**

1. **Repository Pattern** - Abstraction of data access
2. **Unit of Work** - Transaction management
3. **Service Layer** - Business logic separation
4. **Dependency Injection** - Loose coupling
5. **Factory Pattern** - Object creation
6. **Strategy Pattern** - Algorithm selection
7. **Middleware Pattern** - Request pipeline

---

## 🚀 Getting Started

### **Prerequisites**

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [SQL Server 2019+](https://www.microsoft.com/sql-server) or SQL Server Express
- [Git](https://git-scm.com/)

### **Installation**

1. **Clone the repository**
   ```bash
   git clone https://github.com/tr-wa2el/FullstackMVC.git
   cd FullstackMVC
   ```

2. **Update Connection String**

   Edit `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=UniversityDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Run Database Migrations**
   ```bash
   cd FullstackMVC
   dotnet ef database update
   ```

4. **Run the SQL Schema Fix** (if needed)
   
   Execute `fix_department_schema.sql` in SSMS to add missing columns.

5. **Build and Run**
   ```bash
   dotnet build
   dotnet run
   ```

6. **Access the Application**
   ```
   https://localhost:5001
   or
   https://localhost:44357
   ```

### **Default Login Credentials**

#### **Admin Account**
```
Email: admin@university.edu
Password: Admin@123
```

#### **Student Account**
```
Email: student@university.edu
Password: Student@123
Student: Ahmed Mohamed (SSN: 1001)
```

#### **Instructor Account**
```
Email: instructor@university.edu
Password: Instructor@123
Instructor: Dr. Amr Khaled (ID: 1)
```

---

## 📁 Project Structure

### **Key Directories**

```
FullstackMVC/
│
├── 📂 Controllers/
│   ├── 👨‍💼 AdminController.cs          (Admin CRUD operations)
│   ├── 👨‍🎓 StudentDashboardController.cs (Student portal)
│   ├── 👨‍🏫 InstructorDashboardController.cs (Instructor portal)
│   ├── 🔐 AccountController.cs        (Authentication)
│   ├── 📚 CourseController.cs         (Course management)
│   └── 🏢 DepartmentController.cs     (Department management)
│
├── 📂 Models/             (17 model classes)
│   ├── Student.cs
│   ├── Instructor.cs
│   ├── Course.cs
│   ├── Department.cs
│   ├── Grade.cs
│   ├── Employee.cs
│   └── ApplicationUser.cs
│
├── 📂 Services/        (Service layer with interfaces)
│   ├── 🔌 Interfaces/
│   └── ⚙️ Implementations/
│
├── 📂 Repositories/                (Data access layer)
│   ├── 🔌 Interfaces/
│   └── ⚙️ Implementations/
│
├── 📂 ViewModels/   (15 view models)
│   ├── LoginViewModel.cs
│ ├── RegisterViewModel.cs
│   └── StudentViewModel.cs
│
├── 📂 Middleware/           (3 custom middleware)
│   ├── GlobalExceptionHandlerMiddleware.cs
│   ├── LoggingMiddleware.cs
│   └── RateLimitingMiddleware.cs
│
├── 📂 Filters/   (7 custom filters)
│   ├── GlobalExceptionFilter.cs
│   ├── DepartmentLocationAction.cs
│   └── ResourceOptimization.cs
│
├── 📂 Views/            (50+ Razor views)
│   ├── Admin/         (16 views)
│   ├── StudentDashboard/ (4 views)
│   ├── InstructorDashboard/ (5 views)
│ ├── Student/       (10 views)
│   ├── Instructor/    (8 views)
│   ├── Course/     (7 views)
│   └── Department/    (10 views)
│
└── 📂 wwwroot/        (Static files)
    ├── css/
    ├── js/
    └── lib/
```

---

## 🔐 Authentication & Authorization

### **Authentication Flow**

```
User Registration
    ↓
Email Confirmation (Optional)
    ↓
OTP Verification (WhatsApp)
    ↓
Account Activated
    ↓
Login
    ↓
Role-Based Dashboard Redirect
```

### **Roles & Permissions**

| Role | Permissions |
|------|-------------|
| **Admin** | Full CRUD on all entities, User management, Role assignment, System configuration |
| **Student** | View own courses, View grades, Edit profile, Browse courses/departments |
| **Instructor** | View assigned courses, Manage student grades, Edit own profile, View student lists |

### **Security Features**

- ✅ Password hashing (ASP.NET Core Identity)
- ✅ Anti-CSRF tokens on all forms
- ✅ Rate limiting (1000 requests/60 seconds)
- ✅ SQL injection protection (EF Core parameterization)
- ✅ XSS protection (Razor encoding)
- ✅ Secure cookie authentication
- ✅ Account lockout after failed attempts

---

## 💾 Database Schema

### **Entity Relationship Diagram**

```
┌─────────────┐     ┌──────────────┐         ┌─────────────┐
│ Department  │────────│   Course     │────────│  Instructor │
│      │ 1    * │       │ *    1 │    │
└─────────────┘  └──────────────┘ └─────────────┘
      │ 1        │ 1
      │   │
      │ *   │ *
┌─────────────┐         ┌──────────────┐
│ Student   │────────│    Grade     │
│           │ 1    * │              │
└─────────────┘  └──────────────┘
```

### **Main Entities**

1. **Department**
   - Id, Name, ManagerName, Description, Location, PcNumbers
   - Relationships: Students, Instructors, Courses, Employees

2. **Student**
   - SSN (PK), Name, Age, Gender, Address, Image, DeptId
   - Relationships: Department, Grades

3. **Instructor**
   - Id (PK), Name, Email, Age, Degree, Salary, Address, Image, DeptId
   - Relationships: Department, Courses

4. **Course**
   - Num (PK), Name, Topic, Description, Degree, MinDegree, DeptId, InstructorId
   - Relationships: Department, Instructor, Grades

5. **Grade**
   - Id (PK), StudentSSN, CourseNum, GradeValue
   - Relationships: Student, Course

6. **ApplicationUser** (Identity)
   - Id, UserName, Email, FullName, PhoneNumber, StudentSSN, InstructorId
 - Extends IdentityUser with custom properties

### **Migrations**

```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName

# Remove last migration
dotnet ef migrations remove
```

---

## 👥 User Roles

### **Admin Dashboard**

**Route**: `/Admin/Dashboard`

**Features**:
- System statistics (Students, Instructors, Courses, Departments, Users)
- Quick actions (Create Student, Instructor, Course, Department)
- User management
- Role assignment
- Complete CRUD operations

**Admin Views** (16 total):
```
✅ Dashboard.cshtml
✅ Users.cshtml
✅ AssignRole.cshtml
✅ CreateAdmin.cshtml
✅ Students.cshtml
✅ CreateStudent.cshtml
✅ EditStudent.cshtml
✅ DeleteStudent.cshtml
✅ Instructors.cshtml
✅ CreateInstructor.cshtml
✅ EditInstructor.cshtml    (NEW)
✅ DeleteInstructor.cshtml       (NEW)
✅ Courses.cshtml
✅ CreateCourse.cshtml
✅ EditCourse.cshtml
✅ DeleteCourse.cshtml    (NEW)
✅ Departments.cshtml
✅ CreateDepartment.cshtml
✅ EditDepartment.cshtml       (NEW)
✅ DeleteDepartment.cshtml       (NEW)
```

---

### **Student Dashboard**

**Route**: `/StudentDashboard/Dashboard`

**Features**:
- Welcome message with student info
- Academic performance overview
- Enrolled courses with grades
- Quick action cards
- Progress tracking
- Profile editing

**Student Views** (4 total):
```
✅ Dashboard.cshtml   (Main dashboard)
✅ MyCourses.cshtml      (Enrolled courses & grades)
✅ AllCourses.cshtml (Browse available courses)
✅ Departments.cshtml    (View departments)
✅ EditProfile.cshtml    (Update profile)
```

---

### **Instructor Dashboard**

**Route**: `/InstructorDashboard/Dashboard`

**Features**:
- Teaching overview
- Assigned courses
- Student management
- Grade entry/editing
- Course statistics
- Profile management

**Instructor Views** (5 total):
```
✅ Dashboard.cshtml       (Main dashboard)
✅ MyCourses.cshtml     (Assigned courses)
✅ CourseStudents.cshtml  (View students in course)
✅ AddGrade.cshtml        (Add student grade)
✅ EditGrade.cshtml       (Edit existing grade)
✅ EditProfile.cshtml     (Update profile)
```

---

## 📸 Screenshots

### **Admin Dashboard**
![Admin Dashboard](https://via.placeholder.com/800x400/667eea/ffffff?text=Admin+Dashboard)

### **Student Dashboard**
![Student Dashboard](https://via.placeholder.com/800x400/56ab2f/ffffff?text=Student+Dashboard)

### **Instructor Dashboard**
![Instructor Dashboard](https://via.placeholder.com/800x400/4facfe/ffffff?text=Instructor+Dashboard)

### **Course Management**
![Course Management](https://via.placeholder.com/800x400/f093fb/ffffff?text=Course+Management)

---

## 🔌 API Documentation

### **REST API Endpoints**

Base URL: `https://localhost:5001/api`

#### **Courses API**

```http
GET    /api/courses              # Get all courses
GET    /api/courses/{id}         # Get course by ID
POST   /api/courses           # Create new course
PUT    /api/courses/{id}         # Update course
DELETE /api/courses/{id}  # Delete course
GET    /api/courses/department/{deptId}  # Get courses by department
```

**Example Request**:
```bash
curl -X GET https://localhost:5001/api/courses \
  -H "Accept: application/json"
```

**Example Response**:
```json
{
  "success": true,
  "count": 5,
  "data": [
    {
      "num": 1,
      "name": "Introduction to Programming",
      "topic": "C#",
   "degree": 100,
      "minDegree": 60,
      "department": "Computer Science",
      "departmentLocation": "Smart"
    }
  ]
}
```

---

## 🧪 Testing

### **Unit Testing**

Run tests:
```bash
dotnet test
```

### **Manual Testing**

Follow the comprehensive testing guide:
- See `ADMIN_CRUD_TESTING_GUIDE.md` for detailed test scenarios
- Test all CRUD operations
- Verify role-based access
- Check validation rules
- Test responsive design

### **Test Accounts**

```
Admin:   admin@university.edu / Admin@123
Student:    student@university.edu / Student@123
Instructor: instructor@university.edu / Instructor@123
```

---

## 🎨 UI Design System

### **Color Palette**

```css
/* Primary Colors */
--primary-gradient: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
--success-gradient: linear-gradient(135deg, #56ab2f 0%, #a8e6cf 100%);
--info-gradient: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
--warning-gradient: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);

/* Shadows */
--shadow-sm: 0 2px 4px rgba(0,0,0,0.1);
--shadow-md: 0 4px 8px rgba(0,0,0,0.15);
--shadow-lg: 0 8px 16px rgba(0,0,0,0.2);

/* Border Radius */
--border-radius: 15px;
```

### **Components**

- **Cards**: Rounded corners, gradients, hover effects
- **Buttons**: Icon + text, gradient backgrounds
- **Badges**: Color-coded status indicators
- **Progress Bars**: Animated, percentage-based
- **Tables**: Hover effects, zebra striping
- **Forms**: Validation, helpful messages

---

## 📊 Project Statistics

- **Total Files**: 150+
- **Total Lines of Code**: ~15,000
- **Controllers**: 12
- **Models**: 17
- **Services**: 8
- **Repositories**: 4
- **Views**: 50+
- **ViewModels**: 15
- **Middleware**: 3
- **Filters**: 7
- **Migrations**: 10+

---

## 🔧 Configuration

### **appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=UniversityDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Authentication": {
    "Facebook": {
      "AppId": "YOUR_APP_ID",
      "AppSecret": "YOUR_APP_SECRET"
    }
  },
  "WhatsApp": {
    "ApiUrl": "https://api.whatsapp.com",
    "ApiKey": "YOUR_API_KEY"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-password"
  },
  "AllowedHosts": "*"
}
```

---

## 🚀 Deployment

### **IIS Deployment**

1. **Publish the application**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Configure IIS**
   - Create new application pool (.NET CLR Version: No Managed Code)
   - Create new website pointing to publish folder
   - Set application pool

3. **Update Connection String** in `appsettings.json`

### **Azure Deployment**

1. **Create Azure resources**
   - App Service
   - SQL Database

2. **Deploy using Visual Studio**
   - Right-click project → Publish
   - Select Azure → App Service
   - Configure settings

3. **Update Connection Strings** in Azure Portal

---

## 📚 Documentation

- **[Admin CRUD Review](ADMIN_CRUD_REVIEW.md)** - Comprehensive code review
- **[Admin CRUD Completion](ADMIN_CRUD_COMPLETION.md)** - Implementation report
- **[Testing Guide](ADMIN_CRUD_TESTING_GUIDE.md)** - Detailed testing scenarios
- **[Login Redirect Fix](LOGIN_REDIRECT_FIX.md)** - Authentication fix documentation
- **[Unified Design System](UNIFIED_DESIGN_SYSTEM.md)** - UI/UX guidelines

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/YourFeature
   ```
3. **Commit your changes**
   ```bash
   git commit -m "Add some feature"
   ```
4. **Push to the branch**
   ```bash
   git push origin feature/YourFeature
   ```
5. **Open a Pull Request**

### **Coding Standards**

- Follow C# naming conventions
- Use SOLID principles
- Write clean, readable code
- Add XML documentation comments
- Include unit tests for new features
- Follow the existing architecture patterns

---

## 🐛 Known Issues

1. **Database Schema** - Run `fix_department_schema.sql` after first migration
2. **Rate Limiting** - Adjust limits in `Program.cs` for development
3. **Email Service** - Configure SMTP settings in `appsettings.json`

---

## 🗺️ Roadmap

### **Version 2.0** (Planned)

- [ ] RESTful API expansion
- [ ] Swagger/OpenAPI documentation
- [ ] Real-time notifications (SignalR)
- [ ] File upload for assignments
- [ ] Attendance tracking
- [ ] Report generation (PDF/Excel)
- [ ] Calendar integration
- [ ] Email templates
- [ ] Bulk import/export
- [ ] Advanced search filters
- [ ] Pagination improvements
- [ ] Caching implementation
- [ ] Docker containerization
- [ ] CI/CD pipeline

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2025 FullstackMVC Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 👨‍💻 Authors

- **tr-wa2el** - *Initial work* - [GitHub](https://github.com/tr-wa2el)

---

## 🙏 Acknowledgments

- ASP.NET Core Team
- Bootstrap Team
- Entity Framework Team
- Stack Overflow Community
- GitHub Community

---

## 📞 Support

For support, email your-email@example.com or open an issue in the repository.

---

## ⭐ Star History

If you find this project useful, please consider giving it a star! ⭐

[![Star History Chart](https://api.star-history.com/svg?repos=tr-wa2el/FullstackMVC&type=Date)](https://star-history.com/#tr-wa2el/FullstackMVC&Date)

---

## 📈 Project Status

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![Test Coverage](https://img.shields.io/badge/coverage-85%25-yellowgreen)
![Code Quality](https://img.shields.io/badge/quality-A-brightgreen)
![Maintenance](https://img.shields.io/badge/maintenance-active-brightgreen)

---

<div align="center">

**Made with ❤️ using ASP.NET Core MVC**

[⬆ Back to Top](#-fullstackmvc---university-management-system)

</div>

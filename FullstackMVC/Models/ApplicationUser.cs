namespace FullstackMVC.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Address { get; set; }

        // Link to Student or Instructor
        [ForeignKey(nameof(Student))]
        public int? StudentSSN { get; set; }

        public Student? Student { get; set; }

        [ForeignKey(nameof(Instructor))]
        public int? InstructorId { get; set; }

        public Instructor? Instructor { get; set; }

        // OTP for WhatsApp verification
        public string? OtpCode { get; set; }

        public DateTime? OtpExpiry { get; set; }

        public bool IsPhoneVerified { get; set; }
    }
}

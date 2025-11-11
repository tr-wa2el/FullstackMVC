namespace FullstackMVC.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class VerifyOtpViewModel
    {
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "OTP code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        [Display(Name = "OTP Code")]
        public string OtpCode { get; set; }

        public string? UserId { get; set; }
    }
}

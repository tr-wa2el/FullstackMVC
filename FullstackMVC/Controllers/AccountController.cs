namespace FullstackMVC.Controllers
{
    using FullstackMVC.Context;
    using FullstackMVC.Models;
    using FullstackMVC.Services.Interfaces;
    using FullstackMVC.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IWhatsAppService _whatsAppService;

        private readonly IEmailService _emailService;

        private readonly CompanyContext _context;

        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IWhatsAppService whatsAppService,
            IEmailService emailService,
            CompanyContext context,
            ILogger<AccountController> logger
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _whatsAppService = whatsAppService;
            _emailService = emailService;
            _context = context;
            _logger = logger;
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Prevent Admin role registration from public registration
                if (model.Role == "Admin")
                {
                    ModelState.AddModelError("Role", "Admin accounts can only be created by existing administrators.");
                    return View(model);
                }

                // Validate Student/Instructor exists based on role
                if (model.Role == "Student" && model.StudentSSN.HasValue)
                {
                    var student = await _context.Students.FindAsync(model.StudentSSN.Value);
                    if (student == null)
                    {
                        ModelState.AddModelError(
                            "StudentSSN",
                            "Student with this SSN does not exist"
                        );
                        return View(model);
                    }
                }
                else if (model.Role == "Instructor" && model.InstructorId.HasValue)
                {
                    var instructor = await _context.Instructors.FindAsync(model.InstructorId.Value);
                    if (instructor == null)
                    {
                        ModelState.AddModelError(
                            "InstructorId",
                            "Instructor with this ID does not exist"
                        );
                        return View(model);
                    }
                }
                else if (string.IsNullOrEmpty(model.Role) || (model.Role != "Student" && model.Role != "Instructor"))
                {
                    ModelState.AddModelError("Role", "Please select either Student or Instructor role.");
                    return View(model);
                }

                // Generate OTP
                var otp = GenerateOtp();

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    StudentSSN = model.Role == "Student" ? model.StudentSSN : null,
                    InstructorId = model.Role == "Instructor" ? model.InstructorId : null,
                    OtpCode = otp,
                    OtpExpiry = DateTime.UtcNow.AddMinutes(10),
                    IsPhoneVerified = false,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign role (Student or Instructor only)
                    await _userManager.AddToRoleAsync(user, model.Role);

                    // Send OTP via WhatsApp
                    await _whatsAppService.SendOtpAsync(model.PhoneNumber, otp);

                    // Generate email confirmation token and send email
                    var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, token = emailToken },
                        protocol: Request.Scheme
                    );

                    await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);

                    TempData["UserId"] = user.Id;
                    TempData["PhoneNumber"] = model.PhoneNumber;
                    TempData["SuccessMessage"] =
                        "Registration successful! Please verify your phone number with the OTP sent to WhatsApp and confirm your email.";

                    return RedirectToAction("VerifyOtp");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: /Account/VerifyOtp
        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyOtp()
        {
            var model = new VerifyOtpViewModel
            {
                UserId = TempData["UserId"]?.ToString(),
                PhoneNumber = TempData["PhoneNumber"]?.ToString(),
            };

            return View(model);
        }

        // POST: /Account/VerifyOtp
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                    return View(model);
                }

                if (user.OtpCode == model.OtpCode && user.OtpExpiry > DateTime.UtcNow)
                {
                    user.IsPhoneVerified = true;
                    user.PhoneNumberConfirmed = true;
                    user.OtpCode = null;
                    user.OtpExpiry = null;

                    await _userManager.UpdateAsync(user);

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["SuccessMessage"] =
                        "Phone verified successfully! Please check your email to confirm your account.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid or expired OTP code");
                }
            }

            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Email confirmed successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Email confirmation failed.";
            }

            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName!,
                        model.Password,
                        model.RememberMe,
                        lockoutOnFailure: true
                    );

                    if (result.Succeeded)
                    {
                        // Get user role and redirect accordingly
                        var roles = await _userManager.GetRolesAsync(user);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        // Custom redirect based on role
                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        else if (roles.Contains("Student"))
                        {
                            return RedirectToAction("Dashboard", "StudentDashboard");
                        }
                        else if (roles.Contains("Instructor"))
                        {
                            return RedirectToAction("Dashboard", "InstructorDashboard");
                        }

                        return RedirectToAction("Index", "Home");
                    }

                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            "Account is locked. Please try again later."
                        );
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
            }

            return View(model);
        }

        // External Login - Facebook
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                provider,
                redirectUrl
            );
            return Challenge(properties, provider);
        }

        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(
            string? returnUrl = null,
            string? remoteError = null
        )
        {
            if (remoteError != null)
            {
                TempData["ErrorMessage"] = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false
            );

            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }

            // If the user does not have an account, then ask them to create one
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Provider"] = info.LoginProvider;

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            return View(
                "ExternalLoginConfirmation",
                new ExternalLoginViewModel { Email = email, FullName = name }
            );
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(
            ExternalLoginViewModel model,
            string? returnUrl = null
        )
        {
            if (ModelState.IsValid)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return RedirectToAction("Login");
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        // Assign default role
                        await _userManager.AddToRoleAsync(user, "Student");

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { userId = user.Id, token = token },
                    protocol: Request.Scheme
                );

                await _emailService.SendPasswordResetAsync(user.Email, resetLink);

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ResetPasswordViewModel { UserId = userId, Token = token };

            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                var result = await _userManager.ResetPasswordAsync(
                    user,
                    model.Token,
                    model.Password
                );

                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Account/Profile
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = string.Join(", ", roles);
            return View(user);
        }

        // Helper methods
        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}

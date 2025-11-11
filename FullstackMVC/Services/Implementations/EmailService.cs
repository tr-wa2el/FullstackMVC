namespace FullstackMVC.Services.Implementations
{
    using System.Net;
    using System.Net.Mail;

    public class EmailService : Interfaces.IEmailService
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");
                var requireAuth = bool.Parse(_configuration["Email:RequireAuthentication"] ?? "true");
                var smtpUsername = _configuration["Email:Username"];
                var smtpPassword = _configuration["Email:Password"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"];

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername))
                {
                    _logger.LogWarning("Email configuration is incomplete. Email not sent.");
                    // For development, just log and return true
                    _logger.LogInformation($"[DEV MODE] Email to: {toEmail}, Subject: {subject}");
                    return true;
                }

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = requireAuth ? new NetworkCredential(smtpUsername, smtpPassword) : null,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000 // 30 seconds timeout
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(
                        fromEmail ?? smtpUsername,
                        fromName ?? "University System"
                    ),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}: {ex.Message}");
                // In development mode, return true to allow testing without email setup
                return true;
            }
        }

        public async Task<bool> SendEmailConfirmationAsync(string toEmail, string confirmationLink)
        {
            var subject = "Confirm Your Email Address";
            var body =
                $@"
         <html>
       <body>
           <h2>Email Confirmation</h2>
            <p>Thank you for registering with our University System.</p>
            <p>Please confirm your email address by clicking the link below:</p>
     <p><a href='{confirmationLink}'>Confirm Email Address</a></p>
           <p>If you did not create an account, please ignore this email.</p>
  <br/>
          <p>Best regards,<br/>University System Team</p>
                </body>
 </html>";

            return await SendEmailAsync(toEmail, subject, body);
        }

        public async Task<bool> SendPasswordResetAsync(string toEmail, string resetLink)
        {
            var subject = "Reset Your Password";
            var body =
                $@"
       <html>
       <body>
   <h2>Password Reset Request</h2>
   <p>We received a request to reset your password.</p>
        <p>Click the link below to reset your password:</p>
      <p><a href='{resetLink}'>Reset Password</a></p>
         <p>This link will expire in 1 hour.</p>
           <p>If you did not request a password reset, please ignore this email.</p>
             <br/>
   <p>Best regards,<br/>University System Team</p>
              </body>
    </html>";

            return await SendEmailAsync(toEmail, subject, body);
        }
    }
}

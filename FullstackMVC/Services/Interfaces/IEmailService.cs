namespace FullstackMVC.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);

        Task<bool> SendEmailConfirmationAsync(string toEmail, string confirmationLink);

        Task<bool> SendPasswordResetAsync(string toEmail, string resetLink);
    }
}

namespace FullstackMVC.Services.Interfaces
{
    public interface IWhatsAppService
    {
        Task<bool> SendOtpAsync(string phoneNumber, string otp);

        Task<bool> SendMessageAsync(string phoneNumber, string message);
    }
}

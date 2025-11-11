namespace FullstackMVC.Services.Implementations
{
    using System.Text;
    using System.Text.Json;

    public class WhatsAppService : Interfaces.IWhatsAppService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IConfiguration _configuration;

        private readonly ILogger<WhatsAppService> _logger;

        public WhatsAppService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<WhatsAppService> logger
        )
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendOtpAsync(string phoneNumber, string otp)
        {
            var message = $"Your verification code is: {otp}. This code will expire in 10 minutes.";
            return await SendMessageAsync(phoneNumber, message);
        }

        public async Task<bool> SendMessageAsync(string phoneNumber, string message)
        {
            try
            {
                var instanceId = _configuration["WhatsApp:InstanceId"];
                var accessToken = _configuration["WhatsApp:AccessToken"];
                var apiUrl = _configuration["WhatsApp:ApiUrl"];

                if (string.IsNullOrEmpty(instanceId) || string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogError("WhatsApp configuration is missing");
                    return false;
                }

                // Remove "+" prefix from phone number if present
                var cleanPhoneNumber = phoneNumber.TrimStart('+');

                var client = _httpClientFactory.CreateClient();

                var requestBody = new
                {
                    number = cleanPhoneNumber,
                    type = "text",
                    message = message,
                    instance_id = instanceId,
                    access_token = accessToken,
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"WhatsApp message sent successfully to {cleanPhoneNumber}");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError(
                        $"Failed to send WhatsApp message. Status: {response.StatusCode}, Error: {errorContent}"
                    );
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception while sending WhatsApp message to {phoneNumber}");
                return false;
            }
        }
    }
}

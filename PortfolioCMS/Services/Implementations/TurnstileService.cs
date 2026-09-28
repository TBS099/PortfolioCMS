using System.Text.Json.Serialization;

namespace PortfolioCMS.Services.Implementations
{
    // Verifies a Cloudflare Turnstile token server-side against Cloudflare's
    // own siteverify endpoint. This must happen in the backend — the
    // frontend's token is just a claim ("a widget said this request passed"),
    // and only Cloudflare (via the secret key) can confirm it's genuine.
    public class TurnstileService
    {
        private const string SiteverifyUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TurnstileService> _logger;

        public TurnstileService(HttpClient httpClient, IConfiguration configuration, ILogger<TurnstileService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> VerifyAsync(string? token, string? remoteIp)
        {
            var secretKey = _configuration["Turnstile:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                // Fail closed: an unconfigured secret means we can't verify
                _logger.LogError("Turnstile:SecretKey is not configured - rejecting contact submission.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(token) || token.Length > 2048)
                return false;

            try
            {
                var formData = new Dictionary<string, string>
                {
                    ["secret"] = secretKey,
                    ["response"] = token,
                };
                if (!string.IsNullOrWhiteSpace(remoteIp))
                    formData["remoteip"] = remoteIp;

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _httpClient.PostAsync(
                    SiteverifyUrl,
                    new FormUrlEncodedContent(formData),
                    cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Turnstile siteverify returned {StatusCode}.", response.StatusCode);
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<TurnstileVerifyResponse>(cts.Token);
                return result?.Success == true;
            }
            catch (Exception ex)
            {
                // Network hiccup, timeout, malformed response, etc. - fail
                // closed rather than letting an unverifiable submission through.
                _logger.LogWarning(ex, "Turnstile verification failed.");
                return false;
            }
        }

        private class TurnstileVerifyResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("error-codes")]
            public List<string>? ErrorCodes { get; set; }
        }
    }
}
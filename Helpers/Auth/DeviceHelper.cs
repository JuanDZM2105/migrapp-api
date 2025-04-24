using System.Security.Cryptography;
using System.Text;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace migrapp_api.Helpers.Auth
{
    public class DeviceHelper : IDeviceHelper
    {
        private readonly IConfiguration _config;
        public DeviceHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateDeviceToken(int userId, string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                userAgent = "unknown-device";

            var secret = _config["Jwt:DeviceSecret"];
            var payload = $"{userId}:{userAgent}";
            var hash = new HMACSHA256(Encoding.UTF8.GetBytes(secret)).ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToBase64String(hash);
        }
    }
}

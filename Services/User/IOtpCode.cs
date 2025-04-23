namespace migrapp_api.Services.User
{
    public interface IOtpCode
    {
        string GenerateOtp(string secretKey);
        bool ValidateOtp(string secretKey, string otpCode);
    }
}

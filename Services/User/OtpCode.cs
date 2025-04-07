using OtpNet;
using System;

namespace CosmoHosting.Services.User
{
    public class OtpService : IOtpCode
    {
        public string GenerateOtp(string secretKey)
        {
            Console.WriteLine($"Generating OTP for {secretKey} secretKey.");

            var secretKeyBytes = Base32Encoding.ToBytes(secretKey);


            var totp = new Totp(secretKeyBytes, step: 300);

            var otpCode = totp.ComputeTotp();
            Console.WriteLine($"Sended OTP (Client): {otpCode}");
            return otpCode;
        }

        public bool ValidateOtp(string secretKey, string otpCode)
        {

            var secretKeyBytes = Base32Encoding.ToBytes(secretKey);



            var totp = new Totp(secretKeyBytes, step: 300);

            Console.WriteLine($"Secret Key: {secretKey}");
            Console.WriteLine($"Received OTP (client): {otpCode}");


            long timeWindowUsed;

            bool isValid = totp.VerifyTotp(otpCode, out timeWindowUsed, new VerificationWindow(1, 1));

            Console.WriteLine($"Is valid?: {isValid}");
            return isValid;
        }
    }
}

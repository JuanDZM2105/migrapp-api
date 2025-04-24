using System.Threading.Tasks;

namespace migrapp_api.Helpers
{
    public interface IEmailHelper
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public interface ISmsHelper
    {
        Task SendSmsAsync(string toPhone, string message);
    }

    public interface IJwtTokenGenerator
    {
        string GenerateToken(string email, string type, int id, bool rememberMe);
    }
}

using System.Threading.Tasks;

namespace migrapp_api.Repositories
{
    public interface IMfaCodeRepository
    {
        Task SaveCodeAsync(string email, string code);
        Task<bool> VerifyCodeAsync(string email, string code);
    }
}
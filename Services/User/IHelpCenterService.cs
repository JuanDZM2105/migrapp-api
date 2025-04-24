using migrapp_api.Models.HelpCenter;

namespace migrapp_api.Services.HelpCenter
{
    public interface IHelpCenterService
    {
        Task<object?> GetHelpCenterContentAsync(string lang = "en");
        Task<bool> SubmitHelpRequestAsync(HelpRequest request);
    }
}

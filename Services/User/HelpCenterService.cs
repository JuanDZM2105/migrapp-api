using System.Text.Json;
using migrapp_api.Models.HelpCenter;

namespace migrapp_api.Services.HelpCenter
{
    public class HelpCenterService : IHelpCenterService
    {
        private readonly IWebHostEnvironment _env;

        public HelpCenterService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<object?> GetHelpCenterContentAsync(string lang = "en")
        {
            var fileName = $"help_center.{lang}.json";
            var path = Path.Combine(_env.ContentRootPath, "statics", fileName);

            if (!File.Exists(path))
                return null;

            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<object>(json);
        }

        public async Task<bool> SubmitHelpRequestAsync(HelpRequest request)
        {
            var folder = Path.Combine(_env.ContentRootPath, "statics", "help_requests");
            Directory.CreateDirectory(folder);

            var filename = $"{Guid.NewGuid()}.json";
            var fullPath = Path.Combine(folder, filename);
            var json = JsonSerializer.Serialize(request);

            await File.WriteAllTextAsync(fullPath, json);
            return true;
        }
    }
}

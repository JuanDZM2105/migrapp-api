namespace migrapp_api.Helpers.Auth
{
    public interface IDeviceHelper
    {
        public string GenerateDeviceToken(int userId, string userAgent);
    }
}

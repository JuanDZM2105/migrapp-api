using System;

namespace migrapp_api.Models
{
    public class UserMfaCode
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime Expiration { get; set; }
    }
}
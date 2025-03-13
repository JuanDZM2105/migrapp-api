using System.ComponentModel.DataAnnotations;

namespace migrapp_api.DTOs
{
    public class UserDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public required string email { get; set; }
        public required string phone { get; set; }
        public required string password { get; set; }
    }
}

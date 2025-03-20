using System.ComponentModel.DataAnnotations;

namespace migrapp_api.DTOs.admin
{
    public class UserCreationDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public string AccountStatus { get; set; }
        public string UserType { get; set; }
    }
}

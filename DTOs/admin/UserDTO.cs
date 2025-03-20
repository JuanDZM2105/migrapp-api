using System.ComponentModel.DataAnnotations;

namespace migrapp_api.DTOs.admin
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime AccountCreated { get; set; }
        public string AccountStatus { get; set; }
        public string UserType { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActiveNow { get; set; }
    }
}

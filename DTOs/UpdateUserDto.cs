namespace migrapp_api.DTOs
{
    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? PhonePrefix { get; set; }
        public string? Phone { get; set; }
        public string? Country { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhonePrefix { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public DateTime? BirthDate { get; set; }
        public string ImageUrl { get; set; }
    }
}

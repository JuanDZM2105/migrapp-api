public class UserProfileDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
    public string Phone { get; set; }
    public string PhonePrefix { get; set; }
    public string UserType { get; set; }
    public string AccountStatus { get; set; }
    public DateTime AccountCreated { get; set; }
    public DateTime? BirthDate { get; set; }
}
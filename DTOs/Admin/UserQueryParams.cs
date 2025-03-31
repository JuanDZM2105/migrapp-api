namespace migrapp_api.DTOs.Admin
{
    public class UserQueryParams
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? UserType { get; set; }
        public string? AccountStatus { get; set; }
        public string? Country { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "name";
        public string SortDirection { get; set; } = "asc";
    }
}

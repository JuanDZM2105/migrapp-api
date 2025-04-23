namespace migrapp_api.DTOs.Admin
{
    public class UserLogQueryParams
    {
        public string? ActionType { get; set; }
        public string? IpAddress { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "actionDate";
        public string SortDirection { get; set; } = "desc";
    }
}

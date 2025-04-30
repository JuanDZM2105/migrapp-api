namespace migrapp_api.DTOs.Admin
{
    public class LogResponseDto
    {
        public List<UserLogDto> Logs { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

namespace migrapp_api.DTOs
{
    public class OnlineUserDto
    {
        public int? Id { get; set; }

        public string? ConnectionId { get; set; }

        public string? Name { get; set; }

        public string? LastName { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsOnline { get; set; }

        public int UnreadCount { get; set; }
    }
}

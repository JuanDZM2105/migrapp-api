namespace migrapp_api.DTOs.Admin
{
    public class BulkEditDto
    {
        public List<int> UserIds { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }
}
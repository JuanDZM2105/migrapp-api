namespace migrapp_api.DTOs.Admin
{
    public class FiltersDto
    {
        public List<string> UserTypes { get; set; }
        public List<string> AccountStatuses { get; set; }
        public List<string> Countries { get; set; }
        public List<bool> IsActiveNow { get; set; }
    }
}

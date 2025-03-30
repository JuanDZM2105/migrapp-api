using System.Collections.Generic;

namespace migrapp_api.DTOs.Admin
{
    public class AvailableColumnsDto
    {
        public List<ColumnVisibilityDto> Columns { get; set; }
    }

    public class ColumnVisibilityDto
    {
        public string Field { get; set; }
        public string Label { get; set; }
    }
}

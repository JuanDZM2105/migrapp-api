namespace migrapp_api.Helpers
{
    public static class ColumnVisibilityHelper
    {
        // Función para convertir la configuración de columnas a un formato adecuado para la tabla
        public static List<string> ParseVisibleColumns(string columns)
        {
            return columns.Split(',').ToList();
        }
    }
}
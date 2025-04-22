namespace migrapp_api.Models
{
    public class ColumnVisibility
    {
        public int Id { get; set; } // Identificador único
        public int UserId { get; set; } // Relación con el usuario
        public string VisibleColumns { get; set; } // Lista de columnas visibles en formato de cadena (puedes usar JSON o una cadena separada por comas)

        // Relación con la entidad User
        public User User { get; set; }
    }
}

using migrapp_api.DTOs.Admin;
using migrapp_api.Repositories;
using System.Threading.Tasks;
using migrapp_api.Models;

namespace migrapp_api.Services.Admin
{
    public class ColumnVisibilityService : IColumnVisibilityService
    {
        private readonly IColumnVisibilityRepository _columnVisibilityRepository;

        public ColumnVisibilityService(IColumnVisibilityRepository columnVisibilityRepository)
        {
            _columnVisibilityRepository = columnVisibilityRepository;
        }

        public async Task<AvailableColumnsDto> GetAvailableColumnsAsync(int userId)
        {
            // Obtener las columnas predeterminadas
            var availableColumns = new List<ColumnVisibilityDto>
            {
                new ColumnVisibilityDto { Field = "name", Label = "Nombre" },
                new ColumnVisibilityDto { Field = "lastName", Label = "Apellido" },
                new ColumnVisibilityDto { Field = "email", Label = "Correo" },
                new ColumnVisibilityDto { Field = "country", Label = "País" },
                new ColumnVisibilityDto { Field = "phone", Label = "Teléfono" },
                new ColumnVisibilityDto { Field = "accountStatus", Label = "Estado de cuenta" },
                new ColumnVisibilityDto { Field = "birthDate", Label = "Fecha de nacimiento" },
                new ColumnVisibilityDto { Field = "accountCreated", Label = "Fecha de creación de cuenta" },
                new ColumnVisibilityDto { Field = "lastLogin", Label = "Última conexión" },
                new ColumnVisibilityDto { Field = "isActiveNow", Label = "Activo ahora" },
            };

            // Obtener la configuración guardada de las columnas visibles
            var userConfig = await _columnVisibilityRepository.GetByUserIdAsync(userId);

            // Si no hay configuración guardada, enviamos las columnas predeterminadas
            if (userConfig == null)
            {
                return new AvailableColumnsDto { Columns = availableColumns };
            }

            // Filtramos las columnas que el usuario ya tiene configuradas
            var visibleColumns = userConfig.VisibleColumns.Split(',');

            var availableColumnsToAdd = availableColumns.Where(c => !visibleColumns.Contains(c.Field)).ToList();

            return new AvailableColumnsDto { Columns = availableColumnsToAdd };
        }


        public async Task SaveColumnVisibilityAsync(int userId, SaveColumnVisibilityDto dto)
        {
            var currentVisibility = await _columnVisibilityRepository.GetByUserIdAsync(userId);

            if (currentVisibility == null)
            {
                // Si no existe, crea una nueva configuración
                currentVisibility = new ColumnVisibility
                {
                    UserId = userId,
                    VisibleColumns = string.Join(",", dto.VisibleColumns) // Concatenar columnas visibles en formato de lista
                };
                await _columnVisibilityRepository.SaveColumnVisibilityAsync(currentVisibility);
            }
            else
            {
                // Si ya existe, actualiza la configuración
                currentVisibility.VisibleColumns = string.Join(",", dto.VisibleColumns);
                await _columnVisibilityRepository.SaveColumnVisibilityAsync(currentVisibility);
            }
        }
    }
}

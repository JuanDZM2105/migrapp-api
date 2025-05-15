using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace migrapp_api.Extenions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name) ?? throw new
                Exception("No se puede obtener el nombre de usuario");

        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : throw new UnauthorizedAccessException();
        }
    }
}

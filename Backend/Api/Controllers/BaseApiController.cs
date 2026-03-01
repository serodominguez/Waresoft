using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        // Obtiene el ID del usuario autenticado desde el token JWT
        protected int AuthenticatedUserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    throw new UnauthorizedAccessException("Usuario no autenticado o token inválido");
                }

                return userId;
            }
        }

        protected string AuthenticatedUserName
        {
            get
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(userName))
                {
                    throw new UnauthorizedAccessException("Nombre de usuario no encontrado en el token");
                }

                return userName;
            }
        }

        protected string AuthenticatedUserRole
        {
            get
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(role))
                {
                    throw new UnauthorizedAccessException("Rol no encontrado en el token");
                }

                return role;
            }
        }

        protected int AuthenticatedUserStoreId
        {
            get
            {
                var storeIdClaim = User.FindFirst("storeId")?.Value;

                if (string.IsNullOrEmpty(storeIdClaim) || !int.TryParse(storeIdClaim, out int storeId))
                {
                    throw new UnauthorizedAccessException("Store ID no encontrado en el token");
                }

                return storeId;
            }
        }

        protected string AuthenticatedUserStoreName
        {
            get
            {
                var storeName = User.FindFirst("storeName")?.Value;

                if (string.IsNullOrEmpty(storeName))
                {
                    throw new UnauthorizedAccessException("Store name no encontrado en el token");
                }

                return storeName;
            }
        }
        protected string AuthenticatedUserStoreType
        {
            get
            {
                var storeType = User.FindFirst("storeType")?.Value;

                if (string.IsNullOrEmpty(storeType))
                {
                    throw new UnauthorizedAccessException("Store type no encontrado en el token");
                }

                return storeType;
            }
        }

        // Verifica si el usuario autenticado tiene un rol específico
        protected bool HasRole(string role)
        {
            return User.IsInRole(role);
        }

        // Obtiene todos los claims del usuario autenticado
        protected IEnumerable<Claim> GetUserClaims()
        {
            return User.Claims;
        }
    }
}

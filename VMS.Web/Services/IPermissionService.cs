using System.Security.Claims;

namespace VMS.Web.Services;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(
        ClaimsPrincipal user,
        string permissionCode);
}
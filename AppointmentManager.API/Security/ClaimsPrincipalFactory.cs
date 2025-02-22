using System.Security.Claims;
using AppointmentManager.API.Models;

namespace AppointmentManager.API.Security;

public static class ClaimsPrincipalFactory
{
    public static ClaimsPrincipal Create(Admin admin)
    {
        var claims = CreateClaimsFromAdmin(admin);
        
        var identity = new ClaimsIdentity(claims);
        
        var principal = new ClaimsPrincipal(identity);

        return principal;
    }

    private static ICollection<Claim> CreateClaimsFromAdmin(Admin admin)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, admin.Name),
            new(ClaimTypes.NameIdentifier, admin.Id.ToString("D")),
            new(ClaimTypes.Email, admin.Email)
        };

        return claims;
    }
}
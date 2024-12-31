using System.Security.Claims;

namespace Infrastructure.Utils.Interfaces
{
    public interface IJwtToken
    {
        string GenerateToken(string username, string role);
        ClaimsPrincipal GetClaimsPrincipalFromToken(string token);
    }
}

using Infrastructure.Utils.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Utils
{
    public class JwtToken : IJwtToken
    {
        private readonly string _secretKey = "your_secret_key";  // Should come from a secure config
        private readonly string _issuer = "your_issuer";  // Issuer of the token
        private readonly string _audience = "your_audience";  // Audience of the token

        public string GenerateToken(string username, string role)
        {
            Claim[] claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
        };

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_secretKey));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),  // Adjust token expiry as needed
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken? jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            return new ClaimsPrincipal(new ClaimsIdentity(jwtToken?.Claims));
        }
    }

}

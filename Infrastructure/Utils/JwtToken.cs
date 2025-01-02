using Infrastructure.Utils.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Resource;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Utils
{
    public class JwtToken : IJwtToken
    {
        private readonly IConfiguration _configuration;
        private readonly IResourceHelper _resourceHelper;
        private const string _jwtempty = "JWT_EMPTY";
        private const string _invalidtoken = "INVALID_TOKEN";
        public JwtToken(IConfiguration configuration, IResourceHelper resourceHelper)
        {
            _configuration = configuration;
            _resourceHelper = resourceHelper;

        }
        public string GenerateToken(string username, string role)
        {
            Claim[] claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
        };
            string? jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException(nameof(jwtKey), _resourceHelper.User(_jwtempty));
            }
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
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
                throw new UnauthorizedAccessException(_resourceHelper.User(_invalidtoken));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(jwtToken?.Claims));
        }
    }

}

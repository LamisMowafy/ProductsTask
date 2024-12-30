using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Utils
{
    public class JWTHelper
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly string _jwtUserIDKey = "_userID";
        private static IConfiguration _configuration;
        public IDbConnection DbConnection { get; set; }
        public JWTHelper(IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            _configuration = configuration;
            _httpContext = httpContext;
        }
        public virtual string GenerateToken(long userID, List<string> roles, bool? distroyToken = false)
        {
            string _JWTKey = _configuration["Jwt:Key"] ?? string.Empty;

            // Add multiple claims to the list
            List<Claim> claims =
            [
                new Claim(_jwtUserIDKey, userID.ToString()),
                // Add roles as separate claims
                .. roles.Select(role => new Claim(ClaimTypes.Role, role)),
            ];

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_JWTKey));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: distroyToken.Value ? DateTime.Now.AddMinutes(-1) : DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpiryInMins"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public long GetUserID()
        {
            if (_httpContext.HttpContext != null)
            {
                string token = Convert.ToString(_httpContext.HttpContext.Request.Headers["Authorization"]);
                string? newToken = token?.Replace("Bearer ", "").Trim();

                if (string.IsNullOrWhiteSpace(newToken))
                {
                    throw new SecurityTokenMalformedException("JWT is missing or malformed.");
                }

                JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(newToken);
                string userID = jwt.Claims.First(c => c.Type == "_userID").Value;
                return Convert.ToInt32(userID);
            }
            return 0;
        }


        public List<string> GetUserRoles()
        {
            if (_httpContext.HttpContext != null)
            {
                string token = Convert.ToString(_httpContext.HttpContext.Request.Headers["Authorization"]);
                string newToken = token.Replace("Bearer ", "");
                JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(newToken);

                // Extract roles from claims
                List<string> roles = jwt.Claims
                               .Where(c => c.Type == ClaimTypes.Role)
                               .Select(c => c.Value)
                               .ToList();

                return roles;
            }
            return [];
        }

        public string GetSchedulerServerPath()
        {
            string uriPath = _configuration["SchedulerServerPath:SyncUserPath"] ?? string.Empty;

            return uriPath;
        }
    }
}

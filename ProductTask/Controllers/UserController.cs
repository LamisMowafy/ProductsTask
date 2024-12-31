using Infrastructure.Utils.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
namespace ProductTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IJwtToken _jwtTokenService;

        public UserController(IJwtToken jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequest)
        {
            // Validate the user's credentials (from DB, etc.)
            if (loginRequest.Username == "admin" && loginRequest.Password == "password")
            {
                // On successful login, generate token
                string token = _jwtTokenService.GenerateToken(loginRequest.Username, "Admin");

                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
    }

}

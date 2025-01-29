using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContainerWatchlistAPI.Model;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ContainerWatchlistAPI.Helper;
using System.Security.Claims;
using System.Text;


namespace AuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)   
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var serverSecret = _configuration["JWT:ServerSecret"];
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            if (request.Password != user.Password)
            {
                return Unauthorized("Invalid email or password.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serverSecret));
          
                var result = new
                {
                    token = GenerateToken(key, user, issuer, audience)
                };
                return Ok(result); 
            
            
           
        }
    
    private string GenerateToken(SecurityKey key, User user, string issuer, string audience)
        {
            var now = DateTime.UtcNow;

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
              
            });

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(
                issuer: issuer,
                audience: audience,
                subject: identity,
                notBefore: now,
                expires: now.Add(TimeSpan.FromHours(1)),
                signingCredentials: signingCredentials);
            var encodedJwt = handler.WriteToken(token);
            return encodedJwt;
        }
 
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

}
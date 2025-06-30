using JwtEx.Data;
using JwtEx.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtEx.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserDbContext _userdbcontext;
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config, UserDbContext userDbContext)
        {
            _config = config;
            _userdbcontext = userDbContext;

        }

        [HttpPost("register")]
        public IActionResult Register(User cred)
        {
            var existingUser = _userdbcontext.Users.Any(u => u.email == cred.email);
            if (existingUser) 
            {
                return BadRequest("Email already exists");
            }

            var hasher = new PasswordHasher<User>();
            var user = new User
            {
                name = cred.name,
                email = cred.email,
                password = cred.password
            };

            user.password = hasher.HashPassword(user, cred.password);
            user.confirmPassword = user.password;

            _userdbcontext.Users.Add(user);
            _userdbcontext.SaveChanges();
            return Ok("User registered successfully");          
        }


        [HttpPost("Login")]
        public IActionResult Login(LoginCredentials cred)
        {
            var user = _userdbcontext.Users.FirstOrDefault(u => u.email == cred.email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.password, cred.password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid credentials");
            }

            var token = GenerateToken(user);
            return Ok(new { token });
        }

        private string GenerateToken(User user)
        {
            var jwt = _config.GetSection("JwtConfig");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.name),  // Add this
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.email),
                new Claim(ClaimTypes.Role, "User")
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

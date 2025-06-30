using JwtEx.Data;
using JwtEx.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace JwtEx.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly UserDbContext _context;
        public ProfileController(UserDbContext context)
        {
            _context = context;
        }

        private int GetUserIdFromToken()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out int Id) ? Id : 0;
        }

        [Authorize]
        [HttpGet]
        [Route("Greet")]
        public IActionResult Greet()
        {
            var userName = User.Identity.Name;
            return Ok($"Hello {userName}, you're authenticated");
        }

        [Authorize]
        [HttpGet]
        [Route("GetProfile")]
        
        public IActionResult GetProfile()
        {

            var userId = GetUserIdFromToken();
            if (userId == 0)
                return Unauthorized("Invalid token or user not found");

            // Get user based on Id
            var user = _context.Users
                .Where(x => x.Id == userId)
                .Select(x => new UserDTO
                {
                    Id = x.Id,
                    Name = x.name,
                    Email = x.email
                })
                .FirstOrDefault();

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [Authorize]
        [HttpPut]
        [Route("Update")]
        
        public IActionResult UpdateProfile([FromBody] UserDTO model)
        {
            var userId = GetUserIdFromToken();

            if (userId == 0)
                return Unauthorized("Invalid token or user not found");

            var existingUser = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            existingUser.name = model.Name;
            existingUser.email = model.Email;

            _context.SaveChanges();
            return Ok("Detail Updated");
        }

        [Authorize]
        [HttpPatch]
        [Route("UpdatePartial")]
        public ActionResult UpdateUserPartial(string email, [FromBody] JsonPatchDocument<User> patchDocument)
        {
            if (patchDocument == null)
                BadRequest();

            var existingUser = _context.Users.Where(s => s.email == email).FirstOrDefault();

            if (existingUser == null)
                return NotFound();

            var user = new User
            {
                Id = existingUser.Id,
                name = existingUser.name,
                email = existingUser.email,
                password = existingUser.password
            };

            patchDocument.ApplyTo(user, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            existingUser.name = user.name;
            existingUser.email = user.email;

            _context.SaveChanges();

            return Ok("Detail Updated");
        }

        [HttpDelete]
        [Route ("Delete")]
        public IActionResult DeleteProfile()
        {
            var userId = GetUserIdFromToken();

            if (userId == 0)
                return Unauthorized("Invalid token or user not found");

            var existingUser = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            _context.Remove(existingUser);
            _context.SaveChanges();
            return Ok("User Delete");
        }
    }
}
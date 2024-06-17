using Microsoft.AspNetCore.Mvc;
using ushopDN.Data;
using ushopDN.Models;
using MongoDB.Driver;
using ushopDN.Services;

namespace ushopDN.Controllers
{
    [Route("api/v1")]

    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public UserController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            // check if user already exists
            var existingUser = await _context.Users.Find(u => u.Email == user.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return BadRequest(new { error = "User already exists" });
            }
            // generate jwt token

            user.Date = DateTime.Now;
            await _context.Users.InsertOneAsync(user);

            var token = _jwtService.GenerateSecurityToken(user);
            return Ok(new { success = true, token = token });

        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] User user)
        {
            var existingUser = await _context.Users.Find(u => u.Email == user.Email).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                return BadRequest(new { error = "User not found" });
            }
            if (existingUser.Password != user.Password)
            {
                return BadRequest(new { error = "Incorrect password" });
            }
            var token = _jwtService.GenerateSecurityToken(existingUser);
            return Ok(new { success = true, token = token });
        }
    }
}
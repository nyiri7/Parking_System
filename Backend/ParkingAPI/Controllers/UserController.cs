using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingAPI.Data;
using ParkingAPI.Data.DTO;
using ParkingAPI.Services;

namespace ParkingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ParkingDbContext _context;
        private readonly PasswordService pwdService = new PasswordService();

        public UserController(ParkingDbContext context)
        {
            _context = context;
        }

        private async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        
        [HttpPost]
        public async Task<ActionResult<String>> CreateUser(UserRegDTO userInp)
        {
            bool userExists = await _context.Users.Where(u => u.Email == userInp.Email).AnyAsync();
            if(userExists)
            {
                return Conflict("User with this email already exists.");
            }

            User user = new User
            {
                Email = userInp.Email,
                Password = userInp.Password,
                Role = "User"
            };
            user.Password = pwdService.HashPassword(userInp.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User created successfully.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<String>> LoginUser(UserLoginDTO userLogin)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userLogin.Email);

            if (existingUser == null || !pwdService.VerifyPassword(userLogin.Password, existingUser.Password))
            {
                return Unauthorized("Invalid email or password.");
            }
            return Ok("Login successful.");
        }
    }
}
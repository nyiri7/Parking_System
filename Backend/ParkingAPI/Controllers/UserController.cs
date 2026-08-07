using Microsoft.AspNetCore.Authorization;
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

        private readonly TokenService _tokenService;

        public UserController(ParkingDbContext context , TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<ActionResult<String>> LoginUser(UserLoginDTO userLogin)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userLogin.Email);

            if (existingUser == null || !pwdService.VerifyPassword(userLogin.Password, existingUser.Password))
            {
                return Unauthorized("Invalid email or password.");
            }


            if (_tokenService == null) 
            {
                return StatusCode(500, "tokenService is null");
            }

            if (existingUser.Id == null) 
            {
                return StatusCode(500, "existingUser.Id is null");
            }

            if (existingUser.Email == null)
            {
                                return StatusCode(500, "existingUser.Email is null");
            }


            if (existingUser.Role == null)
            {
                                return StatusCode(500, "existingUser.Role is null");
            }


            return Ok(new {token = _tokenService.GenerateToken(existingUser.Id.ToString(), existingUser.Email, existingUser.Role)});
        }
    }
}
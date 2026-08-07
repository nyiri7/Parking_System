using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingAPI.Data;
using ParkingAPI.Data.DTO;

namespace ParkingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ParkingDbContext _context;
        private readonly TokenService _tokenService;

        public CarsController(ParkingDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CarDto>>> GetAllCars()
        {
            var userIdFromToken = _tokenService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            if (userIdFromToken == null)
            {
                return Unauthorized("You are not authorized to create a car for this user.");
            }
            return await _context.Cars
                .AsNoTracking()
                .Select(car => new CarDto
                {
                    Id = car.Id,
                    LicensePlate = car.LicensePlate,
                    Model = car.Model,
                    Brand = car.Brand,
                    UserId = car.UserId
                }).Where(car => car.UserId == Int32.Parse(userIdFromToken))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CarDto>> GetCar(int id)
        {
            var userIdFromToken = _tokenService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            if (userIdFromToken == null)
            {
                return Unauthorized("You are not authorized to create a car for this user.");
            }
            var car = await _context.Cars.Where(car => car.Id == id && car.UserId == Int32.Parse(userIdFromToken))
                .Select(car => new CarDto
                {
                    Id = car.Id,
                    LicensePlate = car.LicensePlate,
                    Model = car.Model,
                    Brand = car.Brand,
                    UserId = car.UserId
                }).FirstOrDefaultAsync();

            if (car == null)
            {
                return NotFound();
            }

            return new CarDto
            {
                Id = car.Id,
                LicensePlate = car.LicensePlate,
                Model = car.Model,
                Brand = car.Brand,
                UserId = car.UserId
            };
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CarDto>> CreateCar(CarDto carDto)
        {
            var userIdFromToken = _tokenService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            if (userIdFromToken == null)
            {
                return Unauthorized("You are not authorized to create a car for this user.");
            }
            var car = new Car
            {
                LicensePlate = carDto.LicensePlate,
                Model = carDto.Model,
                Brand = carDto.Brand,
                UserId = Int32.Parse(userIdFromToken)
            };
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            carDto.Id = car.Id;
            return StatusCode(201, carDto);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var userIdFromToken = _tokenService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            if (userIdFromToken == null)
            {
                return Unauthorized("You are not authorized to create a car for this user.");
            }
            var car = await _context.Cars.Where(car => car.Id == id && car.UserId == Int32.Parse(userIdFromToken))
                .FirstOrDefaultAsync();
            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return Ok("Car deleted successfully.");
        }
    }
}
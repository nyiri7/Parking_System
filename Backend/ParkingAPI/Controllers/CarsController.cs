using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public CarsController(ParkingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarDto>>> GetAllCars()
        {
            return await _context.Cars
                .AsNoTracking()
                .Select(car => new CarDto
                {
                    Id = car.Id,
                    LicensePlate = car.LicensePlate,
                    Model = car.Model,
                    Brand = car.Brand,
                    UserId = car.UserId
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);

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
        public async Task<ActionResult<CarDto>> CreateCar(CarDto carDto)
        {

            var car = new Car
            {
                LicensePlate = carDto.LicensePlate,
                Model = carDto.Model,
                Brand = carDto.Brand,
                UserId = carDto.UserId
            };
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            carDto.Id = car.Id;

            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, carDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
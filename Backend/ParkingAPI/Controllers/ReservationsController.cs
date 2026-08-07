using Microsoft.AspNetCore.Mvc;
using ParkingAPI.Data;
using Microsoft.EntityFrameworkCore;
using ParkingAPI.Data.DTO;
using Microsoft.AspNetCore.Authorization;

namespace ParkingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ParkingDbContext _context;
        private readonly TokenService _tokenService;

        public ReservationsController(ParkingDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpGet("available-spots")]
        [Authorize]
        public async Task<ActionResult<List<SpotCountDTO>>> GetAvailableSpotCounts([FromQuery] DateTime fromTime, [FromQuery] DateTime toTime)
        {
            if (fromTime >= toTime)
            {
                return BadRequest("The 'from' time must be earlier than the 'to' time.");
            }

            var availableSpotCounts = await _context.ParkingSpots
                .Where(spot => !_context.ParkingReservations.Any(res => 
                    res.SpotId == spot.Id && 
                    res.FromTime < toTime && 
                    res.ToTime > fromTime))
                .GroupBy(p => p.Type.Type)
                .Select(g => new SpotCountDTO
                {
                    Type = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(availableSpotCounts);
        }

        [HttpPost("reserve")]
        [Authorize]
        public async Task<IActionResult> MakeReservation([FromBody] CreateReservationDTO request)
        {
            var userIdFromToken = _tokenService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            if (userIdFromToken == null)
            {
                return Unauthorized("You are not authorized to create a car for this user.");
            }

            if (request.FromTime >= request.ToTime)
            {
                return BadRequest("The 'from' time must be earlier than the 'to' time.");
            }

            var availableSpot = await _context.ParkingSpots
                .Where(spot => spot.Type.Type == request.SpotType)
                .Where(spot => !_context.ParkingReservations.Any(res => 
                    res.SpotId == spot.Id && 
                    res.FromTime < request.ToTime && 
                    res.ToTime > request.FromTime))
                .FirstOrDefaultAsync();

            if (availableSpot == null)
            {
                return NotFound($"No available spots of type '{request.SpotType}' for the selected time window.");
            }
            var varCarId = request.CarId;
            if(request.CarId != 0)
            {
                var car_exists = await _context.Cars.AnyAsync(c => c.Id == request.CarId && c.UserId == Int32.Parse(userIdFromToken));
                if (!car_exists)
                    {
                        return NotFound($"No car found with ID {request.CarId} for the current user.");
                    }
            }else
            {
                varCarId = null;
            }

            var newReservation = new ParkingReservation
            {
                FromTime = request.FromTime,
                ToTime = request.ToTime,
                SpotId = availableSpot.Id,
                UserId = Int32.Parse(userIdFromToken),
                CarId = varCarId
            };

            _context.ParkingReservations.Add(newReservation);
            await _context.SaveChangesAsync();

            return Ok("Created"); 
        }

        [HttpDelete("cancel/{reservationId}")]
        [Authorize]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            var userIdFromToken = _tokenService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            if (userIdFromToken == null)
            {
                return Unauthorized("You are not authorized to create a car for this user.");
            }
            var reservation = await _context.ParkingReservations.Where(r => r.Id == reservationId && r.UserId == Int32.Parse(userIdFromToken)).FirstOrDefaultAsync();
            if (reservation == null)
            {
                return NotFound($"No reservation found with ID {reservationId}.");
            }

            _context.ParkingReservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok($"Reservation with ID {reservationId} has been canceled.");
        }


        [HttpGet("my-reservations")]
        [Authorize]
        public async Task<ActionResult<List<ReservationDTO>>> GetMyReservations()
        {
            var userIdFromToken = _tokenService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            if (userIdFromToken == null)
            {
                return Unauthorized("You are not authorized to view reservations for this user.");
            }

            var reservations = await _context.ParkingReservations
                .Where(r => r.UserId == Int32.Parse(userIdFromToken))
                .Include(r => r.Spot)
                .Include(r => r.Car)
                .Select(r => new ReservationDTO
                {
                    Id = r.Id,
                    FromTime = r.FromTime,
                    ToTime = r.ToTime,
                    SpotId = r.Spot.Id,
                    UserId = r.UserId,
                    CarId = r.CarId
                })
                .ToListAsync();

            return Ok(reservations);
        }
    }
}
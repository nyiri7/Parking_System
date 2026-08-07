using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingAPI.Data;
using ParkingAPI.Data.DTO;

namespace ParkingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotController : ControllerBase
    {
        private readonly ParkingDbContext _context;

        public SpotController(ParkingDbContext context)
        {
            _context = context;
        }

        private async Task<List<SpotCountDTO>> GetSpotCounts()
        {
            return await _context.ParkingSpots
                .GroupBy(p => p.Type.Type)
                .Select(g => new SpotCountDTO
                {
                    Type = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<SpotCountDTO>>> GetAllSpots()
        {
            return await GetSpotCounts();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<String>> ModifySpots(List<SpotCountDTO> spotCountDTOinp)
        {
            var currentSpotsInDB = await GetSpotCounts();
            foreach(var currSpotCount in spotCountDTOinp)
            {
                var spotInDB = currentSpotsInDB.FirstOrDefault(s => s.Type == currSpotCount.Type);
                if (spotInDB != null)
                {
                    if (currSpotCount.Count > spotInDB.Count)
                    {
                        var spotsToAdd = currSpotCount.Count - spotInDB.Count;
                        for (int i = 0; i < spotsToAdd; i++)
                        {
                            var newSpot = new ParkingSpot
                            {
                                TypeId = _context.ParkingSpotTypes.FirstOrDefault(s => s.Type == currSpotCount.Type).Id,
                                Available = true
                            };
                            _context.ParkingSpots.Add(newSpot);
                        }
                    }
                    else if (currSpotCount.Count < spotInDB.Count)
                    {
                        var spotsToRemove = spotInDB.Count - currSpotCount.Count;
                        var unoccupiedSpots = await _context.ParkingSpots
                            .Where(s => s.Type.Type == currSpotCount.Type && s.Available == true)
                            .Take(spotsToRemove)
                            .ToListAsync();

                        if (unoccupiedSpots.Count < spotsToRemove)
                        {
                            return BadRequest($"Not enough unoccupied {currSpotCount.Type} spots to remove.");
                        }

                        _context.ParkingSpots.RemoveRange(unoccupiedSpots);
                    }
                }
                else
                {
                    _context.ParkingSpots.AddRange(Enumerable.Range(0, currSpotCount.Count).Select(_ => new ParkingSpot
                    {
                        TypeId = _context.ParkingSpotTypes.FirstOrDefault(s => s.Type == currSpotCount.Type).Id,
                        Available = true
                    }));
                }
            }
            await _context.SaveChangesAsync();
            return Ok("Spot counts modified successfully.");
        }

        [HttpPost("SpotType")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<String>> AddSpotType(SpotTypeDTO spotTypeDTOinp)
        {
            var existingType = await _context.ParkingSpotTypes.FirstOrDefaultAsync(s => s.Type == spotTypeDTOinp.Type);
            if (existingType != null)
            {
                return Conflict("Spot type already exists.");
            }

            var newSpotType = new ParkingSpotType
            {
                Type = spotTypeDTOinp.Type
            };
            _context.ParkingSpotTypes.Add(newSpotType);
            await _context.SaveChangesAsync();

            return Ok("Spot type added successfully.");
        }

        [HttpDelete("SpotType/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<String>> DeleteSpotType(int id)
        {
            var spotType = await _context.ParkingSpotTypes.FindAsync(id);
            if (spotType == null)
            {
                return NotFound("Spot type not found.");
            }

            var associatedSpots = await _context.ParkingSpots.Where(s => s.TypeId == id).ToListAsync();
            if (associatedSpots.Any())
            {
                return BadRequest("Cannot delete spot type with associated parking spots.");
            }

            _context.ParkingSpotTypes.Remove(spotType);
            await _context.SaveChangesAsync();

            return Ok("Spot type deleted successfully.");
        }

        [HttpGet("SpotType")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ParkingSpotType>>> GetSpotTypes()
        {
            return await _context.ParkingSpotTypes.ToListAsync();
        }

    }
}
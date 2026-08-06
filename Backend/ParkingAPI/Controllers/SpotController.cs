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
        public async Task<ActionResult<IEnumerable<SpotCountDTO>>> GetAllSpots()
        {
            return await GetSpotCounts();
        }
        [HttpPost]
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
            return Ok("Spot counts modified successfully.");
        }


    }
}
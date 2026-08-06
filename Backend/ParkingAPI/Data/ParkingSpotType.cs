using System;
using System.Collections.Generic;

namespace ParkingAPI.Data;

public partial class ParkingSpotType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<ParkingSpot> ParkingSpots { get; set; } = new List<ParkingSpot>();
}

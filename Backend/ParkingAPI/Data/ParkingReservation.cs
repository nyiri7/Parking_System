using System;
using System.Collections.Generic;

namespace ParkingAPI.Data;

public partial class ParkingReservation
{
    public int Id { get; set; }

    public DateTime FromTime { get; set; }

    public DateTime ToTime { get; set; }

    public int SpotId { get; set; }

    public int UserId { get; set; }

    public int? CarId { get; set; }

    public virtual Car? Car { get; set; }

    public virtual ParkingSpot Spot { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

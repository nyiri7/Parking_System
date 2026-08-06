using System;
using System.Collections.Generic;

namespace ParkingAPI.Data;

public partial class ParkingSpot
{
    public int Id { get; set; }

    public int? TypeId { get; set; }

    public bool? Available { get; set; }

    public virtual ICollection<ParkingReservation> ParkingReservations { get; set; } = new List<ParkingReservation>();

    public virtual ParkingSpotType? Type { get; set; }
}

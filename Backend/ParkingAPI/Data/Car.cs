using System;
using System.Collections.Generic;

namespace ParkingAPI.Data;

public partial class Car
{
    public int Id { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string? Model { get; set; }

    public string? Brand { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<ParkingReservation> ParkingReservations { get; set; } = new List<ParkingReservation>();

    public virtual User User { get; set; } = null!;
}

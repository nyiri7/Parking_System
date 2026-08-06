using System;
using System.Collections.Generic;

namespace ParkingAPI.Data;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<ParkingReservation> ParkingReservations { get; set; } = new List<ParkingReservation>();
}

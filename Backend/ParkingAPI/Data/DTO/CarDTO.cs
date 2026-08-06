namespace ParkingAPI.Data.DTO;

public class CarDto
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string? Model { get; set; }
    public string? Brand { get; set; }
    public int UserId { get; set; }
}
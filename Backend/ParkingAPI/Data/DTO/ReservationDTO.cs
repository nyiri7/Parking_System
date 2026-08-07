namespace ParkingAPI.Data.DTO
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public int SpotId { get; set; }
        public int UserId { get; set; }
        public int? CarId { get; set; }
    }
}
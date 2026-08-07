
namespace ParkingAPI.Data.DTO
{
    public class SpotCountDTO
    {
        public string Type { get; set; }
        public int Count { get; set; }
    }

    public class SpotTypeDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class CreateReservationDTO
    {
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public string SpotType { get; set; }
        public int UserId { get; set; }
        public int? CarId { get; set; }
    }
}


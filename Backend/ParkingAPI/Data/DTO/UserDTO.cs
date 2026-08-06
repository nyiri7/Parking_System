namespace ParkingAPI.Data.DTO
{
    public class UserRegDTO
    {
        public string Password { get; set; }
        public string PasswordAgain { get; set; }
        public string Email { get; set; }
    }

    public class UserLoginDTO
    {
        public string Password { get; set; }
        public string Email { get; set; }
    }

}
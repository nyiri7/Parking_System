using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ParkingAPI.Services;

namespace ParkingAPI.Tests
{
    [TestFixture]
    public class TokenServiceTests
    {
        private TokenService _tokenService;

        [SetUp]
        public void Setup()
        {

            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "YourSuperSecretKeyThatIsAtLeast32BytesLong!"},
                {"Jwt:ExpireMinutes", "60"},
                {"Jwt:Issuer", "localhost"},
                {"Jwt:Audience", "ParkingSystemAPI"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new TokenService(configuration);
        }

        [Test]
        public void GenerateTokenShouldReturnValidToken()
        {
            string userId = "12345";
            string email = "user@example.com";
            string role = "Admin";

            string token = _tokenService.GenerateToken(userId, email, role);

            Assert.That(token, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ValidateTokenWithCorrectTokenShouldNotReturnNull()
        {

            string userId = "12345";
            string email = "user@example.com";
            string role = "Admin";
            string token = _tokenService.GenerateToken(userId, email, role);

            ClaimsPrincipal? principal = _tokenService.ValidateToken(token);

            Assert.That(principal, Is.Not.Null);
        }

        [Test]
        public void ValidateTokenWithInvalidTokenShouldReturnNull()
        {
            string invalidToken = "Invalid";

            ClaimsPrincipal? principal = _tokenService.ValidateToken(invalidToken);

            Assert.That(principal, Is.Null);
        }

        [Test]
        public void GetUserIdFromToken_WithValidToken_ShouldReturnCorrectUserId()
        {
            string expectedUserId = "999";
            string token = _tokenService.GenerateToken(expectedUserId, "test@test.com", "User");

            string? extractedUserId = _tokenService.GetUserIdFromToken(token);

            Assert.That(extractedUserId, Is.EqualTo(expectedUserId));
        }
    }
}
using NUnit.Framework;
using ParkingAPI.Services;

namespace ParkingAPI.Tests
{
    [TestFixture]
    public class PasswordServiceTests
    {
        private PasswordService _passwordService;

        [SetUp]
        public void Setup()
        {
            _passwordService = new PasswordService();
        }

        [Test]
        public void HashPassword_ShouldReturnValidBCryptHash()
        {
            string plainPassword = "SuperSecretPassword123!";

            string hashedPassword = _passwordService.HashPassword(plainPassword);

            Assert.That(hashedPassword, Is.Not.Null.And.Not.Empty);
            Assert.That(hashedPassword, Is.Not.EqualTo(plainPassword));

            Assert.That(hashedPassword.StartsWith("$2"), Is.True, "The hash should be a valid BCrypt string.");
        }

        [Test]
        public void HashPassword_SamePassword_ShouldReturnDifferentHashes()
        {

            string plainPassword = "SuperSecretPassword123!";


            string hash1 = _passwordService.HashPassword(plainPassword);
            string hash2 = _passwordService.HashPassword(plainPassword);

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {

            string plainPassword = "SuperSecretPassword123!";
            string hashedPassword = _passwordService.HashPassword(plainPassword);


            bool isValid = _passwordService.VerifyPassword(plainPassword, hashedPassword);

            Assert.That(isValid, Is.True);
        }

        [Test]
        public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            string plainPassword = "SuperSecretPassword123!";
            string wrongPassword = "WrongPassword999!";
            string hashedPassword = _passwordService.HashPassword(plainPassword);

            bool isValid = _passwordService.VerifyPassword(wrongPassword, hashedPassword);


            Assert.That(isValid, Is.False);
        }
    }
}
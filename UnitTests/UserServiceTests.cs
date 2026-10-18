using FitRadar.Data;
using FitRadar.Repositories.Interfaces;
using FitRadar.Services;
using FitRadar.Services.Exceptions;
using FitRadar.Services.Interfaces;
using FitRadar.Shared.DTOs.Auth;
using FitRadar.Shared.Models;
using FitRadar.Shared.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace UnitTests
{
    [TestFixture]
    public class Tests
    {
        // Mock<T> (nie samo .Object), żeby w testach móc robić Setup/Verify
        private Mock<UserManager<User>> _userManager = null!;
        private Mock<IEmailSender> _emailSender = null!;
        private FitRadarDbContext _context = null!;
        private UserService _sut = null!;   // sut = system under test

        [SetUp]
        public void Setup()
        {
            _userManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null);

            _emailSender = new Mock<IEmailSender>();

            // Prawdziwy DbContext w pamięci, osobna baza na każdy test
            var options = new DbContextOptionsBuilder<FitRadarDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new FitRadarDbContext(options);

            _sut = new UserService(
                new Mock<IUserRepository>().Object,
                _userManager.Object,
                _emailSender.Object,
                new Mock<IConfiguration>().Object,
                new Mock<ILogger<UserService>>().Object,
                _context,
                new Mock<IHttpClientFactory>().Object,
                new JwtSettings
                {
                    SecretKey = new string('x', 32),
                    Issuer = "test",
                    Audience = "test"
                });
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase("bez-malpy")]
        public void Register_InvalidEmail_ThrowsError(string email)
        {
            // Arrange
            RegisterRequest request = new RegisterRequest
            (
                email, // no @ in email
                "ValidPassword123!",
                "woj",
                "Test"
            );


            // Act
            var response = _sut.RegisterAsync(request);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await response);
        }

        // --- Login ---

        [Test]
        public void Login_UnknownEmail_ThrowsUserNotFound()
        {
            // Arrange
            _userManager
                .Setup(m => m.FindByEmailAsync("x@x.pl"))
                .ReturnsAsync((User?)null);
            var request = new LoginRequest("x@x.pl", "pw");

            // Act
            AsyncTestDelegate act = () => _sut.LoginAsync(request);

            // Assert
            Assert.ThrowsAsync<UserNotFoundException>(act);
            // dla nieznanego użytkownika hasło nie powinno być w ogóle sprawdzane
            _userManager.Verify(
                m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsTokensAndSavesRefreshToken()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "a@a.pl",
                Email = "a@a.pl",
                FirstName = "Jan",
                LastName = "Kowalski",
                EmailConfirmed = true
            };
            _userManager
                .Setup(m => m.FindByEmailAsync("a@a.pl"))
                .ReturnsAsync(user);
            _userManager
                .Setup(m => m.CheckPasswordAsync(user, "ValidPassword123!"))
                .ReturnsAsync(true);
            _userManager
                .Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync((IList<string>)new List<string> { Roles.User });
            var request = new LoginRequest("a@a.pl", "ValidPassword123!");

            // Act
            var result = await _sut.LoginAsync(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Token, Is.Not.Empty);
                Assert.That(new JwtSecurityTokenHandler().CanReadToken(result.Token), Is.True);
                Assert.That(result.RefreshToken, Is.Not.Empty);
                Assert.That(result.User.Email, Is.EqualTo("a@a.pl"));
                Assert.That(_context.Set<RefreshToken>().Count(rt => rt.UserId == user.Id), Is.EqualTo(1));
            });
        }
    }
}
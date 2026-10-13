using Azure.Core;
using FitRadar.Data;
using FitRadar.Repositories.Interfaces;
using FitRadar.Services;
using FitRadar.Services.Interfaces;
using FitRadar.Shared.DTOs.Auth;
using FitRadar.Shared.Models;
using FitRadar.Shared.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Net.Http;

namespace UnitTests
{
    [TestFixture]
    public class Tests
    {
        private UserManager<User> _userManager = null!;
        private IEmailSender _emailSender = null!;
        private UserService _sut = null!;   // sut = system under test

        [SetUp]
        public void Setup()
        {
            _userManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null).Object;

            _emailSender = new Mock<IEmailSender>().Object;

            _sut = new UserService(
                new Mock<IUserRepository>().Object,
                _userManager,
                _emailSender,
                new Mock<IConfiguration>().Object,
                new Mock<ILogger<UserService>>().Object,
                null!,
                new Mock<IHttpClientFactory>().Object,
                new JwtSettings
                {
                    SecretKey = new string('x', 32),
                    Issuer = "test",
                    Audience = "test"
                });
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
    }
}

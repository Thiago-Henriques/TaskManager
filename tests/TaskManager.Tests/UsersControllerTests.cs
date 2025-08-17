using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TaskManager.Api.Controllers;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _jwtServiceMock = new Mock<IJwtService>();
            _controller = new UsersController(_userServiceMock.Object, _jwtServiceMock.Object);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "test@test.com", Name = "Test" };
            _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(user);

            var result = await _controller.GetById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.GetById(userId);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Register_ReturnsCreatedAtAction_WhenUserIsValid()
        {
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com", Name = "Test" };
            _userServiceMock.Setup(s => s.AddUserAsync(user)).Returns(Task.CompletedTask);

            var result = await _controller.Register(user);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(user, createdResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenArgumentException()
        {
            var user = new User { Id = Guid.NewGuid(), Email = "", Name = "Test" };
            _userServiceMock.Setup(s => s.AddUserAsync(user)).Throws(new ArgumentException("Email is required"));

            var result = await _controller.Register(user);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Email is required", badRequest.Value);
        }

        [Fact]
        public async Task Login_ReturnsOk_WithToken_WhenCredentialsAreValid()
        {
            var loginRequest = new LoginRequest { Email = "test@test.com", Password = "123" };
            var user = new User { Id = Guid.NewGuid(), Email = loginRequest.Email, Name = "Test" };
            var token = "fake-jwt-token";

            _userServiceMock.Setup(s => s.LoginAsync(loginRequest.Email, loginRequest.Password)).ReturnsAsync(user);
            _jwtServiceMock.Setup(s => s.GenerateToken(user)).Returns(token);

            var result = await _controller.Login(loginRequest);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var authResponse = Assert.IsType<AuthResponse>(okResult.Value);
            Assert.Equal(token, authResponse.Token);
            Assert.Equal(user.Id, authResponse.UserId);
            Assert.Equal(user.Email, authResponse.Email);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            var loginRequest = new LoginRequest { Email = "test@test.com", Password = "wrong" };
            _userServiceMock.Setup(s => s.LoginAsync(loginRequest.Email, loginRequest.Password)).ReturnsAsync((User)null);

            var result = await _controller.Login(loginRequest);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid credentials", unauthorized.Value);
        }
    }
}
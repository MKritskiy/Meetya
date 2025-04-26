using Xunit;
using Moq;
using Users.Infrastructure.Services;
using Users.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Users.Application.Interfaces;
using Users.Domain.Entities;
using Users.Application.Models;
using Users.Application.Exceptions;
using Events.Application.Interfaces;
using Events.Domain.Entities;
using Events.Application;
using Events.Infrastructure.Services;
using Events.Application.Models;
using Events.Web.Controllers;
using Messages.Application.Interfaces;
using Messages.Domain.Entities;
using Messages.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Messages.Infrastructure.Services;
using Messages.Application.Event;
using Messages.Application.Models;

namespace Users.Tests
{
    public class UserServiceTests
    {
        #region UserService Unit Tests

        [Fact]
        public async Task CreateUser_ShouldReturnUserId_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>())).ReturnsAsync(1);

            var tokenServiceMock = new Mock<ITokenService>();
            var encryptMock = new Mock<IEncrypt>();
            var codeCacheMock = new Mock<ICodeCacheService>();
            var codeGeneratorMock = new Mock<ICodeGenerator>();
            var notificationQueueMock = new Mock<INotificationQueueService>();

            var userService = new UserService(
                userRepositoryMock.Object,
                tokenServiceMock.Object,
                encryptMock.Object,
                codeCacheMock.Object,
                codeGeneratorMock.Object,
                notificationQueueMock.Object
            );

            var user = new User { Email = "test@example.com", Password = "password" };

            // Act
            var result = await userService.CreateUser(user);

            // Assert
            Assert.Equal(1, result);
            userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldReturnAfterAuthDto_WhenCredentialsAreValid()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync("test@example.com"))
                .ReturnsAsync(new User { Id = 1, Email = "test@example.com", Password = "hashedPassword", Salt = "salt", Verified = true });

            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(service => service.GenerateToken(It.IsAny<User>())).Returns("token");

            var encryptMock = new Mock<IEncrypt>();
            encryptMock.Setup(encrypt => encrypt.HashPassword("password", "salt")).Returns("hashedPassword");

            var userService = new UserService(
                userRepositoryMock.Object,
                tokenServiceMock.Object,
                encryptMock.Object,
                null,
                null,
                null
            );

            var loginDto = new Application.Models.LoginDto { Email = "test@example.com", Password = "password" };

            // Act
            var result = await userService.Login(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("token", result.Token);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task Register_ShouldThrowDuplicateEmailException_WhenEmailAlreadyExists()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync("test@example.com"))
                .ReturnsAsync(new User { Id = 1, Email = "test@example.com", Verified = true });

            var userService = new UserService(
                userRepositoryMock.Object,
                null,
                null,
                null,
                null,
                null
            );

            var regDto = new Application.Models.RegDto { Email = "test@example.com", Password = "password" };

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateEmailException>(() => userService.Register(regDto));
        }

        #endregion

        #region ProfileService Unit Tests

        [Fact]
        public async Task AddProfile_ShouldReturnProfileId_WhenProfileIsAddedSuccessfully()
        {
            // Arrange
            var profileRepositoryMock = new Mock<IProfileRepository>();
            profileRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.Profile>())).ReturnsAsync(1);

            var profileService = new ProfileService(profileRepositoryMock.Object);
            var profile = new Domain.Entities.Profile { Id = 1, UserId = 1 };

            // Act
            var result = await profileService.AddProfile(profile);

            // Assert
            Assert.Equal(1, result);
            profileRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.Profile>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProfile_ShouldThrowInvalidOperationException_WhenProfileDoesNotExist()
        {
            // Arrange
            var profileRepositoryMock = new Mock<IProfileRepository>();
            profileRepositoryMock.Setup(repo => repo.DeleteByIdAsync(1)).ReturnsAsync(false);

            var profileService = new ProfileService(profileRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => profileService.DeleteProfile(1));
        }

        [Fact]
        public async Task GetProfileById_ShouldReturnProfile_WhenProfileExists()
        {
            // Arrange
            var profileRepositoryMock = new Mock<IProfileRepository>();
            profileRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(new Domain.Entities.Profile { Id = 1, UserId = 1 });

            var profileService = new ProfileService(profileRepositoryMock.Object);

            // Act
            var result = await profileService.GetProfileById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task RegisterUser_ShouldCreateUserAndReturnOk_WhenDataIsValid()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
            userRepository.Setup(repo => repo.AddAsync(It.IsAny<User>())).ReturnsAsync(1);

            var userService = new UserService(
                userRepository.Object,
                new Mock<ITokenService>().Object,
                new Mock<IEncrypt>().Object,
                new Mock<ICodeCacheService>().Object,
                new Mock<ICodeGenerator>().Object,
                new Mock<INotificationQueueService>().Object
            );

            var userController = new UserController(userService);
            var regDto = new Application.Models.RegDto { Email = "test@example.com", Password = "password" };

            // Act
            var result = await userController.Register(regDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task CreateEvent_ShouldReturnEventId_WhenEventIsCreatedSuccessfully()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IEventRepository>();
            eventRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Events.Domain.Entities.Event>())).ReturnsAsync(1);

            var usersApiClientMock = new Mock<IUsersApiClient>();
            var eventService = new EventService(eventRepositoryMock.Object, usersApiClientMock.Object, null);
            var eventController = new EventController(eventService, eventRepositoryMock.Object);

            var addEventDto = new Events.Application.Models.AddEventDto { Name = "Test Event", Time = DateTime.UtcNow, CreatorId = 1 };

            // Act
            var result = await eventController.AddEvent(addEventDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, okResult.Value);
        }

        [Fact]
        public async Task SendMessage_ShouldAddMessageAndNotifyClients_WhenDataIsValid()
        {
            // Arrange
            var messageRepositoryMock = new Mock<IMessageRepository>();
            messageRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Message>())).ReturnsAsync(1);

            var usersApiClientMock = new Mock<Messages.Application.Users.IUsersApiClient>();
            usersApiClientMock.Setup(client => client.ApiProfileAsync(1))
                .ReturnsAsync(new Messages.Application.Users.Profile { Id = 1 });

            var eventsClientApiMock = new Mock<IEventsClientApi>();
            var messageService = new MessageService(messageRepositoryMock.Object, usersApiClientMock.Object);

            var hubClientsMock = new Mock<IHubCallerClients>();
            var clientProxyMock = new Mock<IClientProxy>();

            hubClientsMock.Setup(c => c.Group("1")).Returns(clientProxyMock.Object);

            var chatHub = new ChatHub(messageService, eventsClientApiMock.Object, usersApiClientMock.Object)
            {
                Clients = hubClientsMock.Object // Устанавливаем свойство Clients
            };

            // Act
            await chatHub.Send(1, "Hello", 1);

            // Assert
            messageRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Message>()), Times.Once);
            clientProxyMock.Verify(client => client.SendCoreAsync(
                It.Is<string>(s => s == "Receive_1"),
                It.Is<object[]>(o => o.Length == 1 && o[0] is MessageDto),
                default), Times.Once);
        }

        #endregion
    }
}
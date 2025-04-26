using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Events.Application.Interfaces;
using Events.Domain.Entities;
using Events.Application.Models;
using Events.Application;
using Events.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Events.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Events.Tests
{
    public class EventsApiTests
    {
        #region EventService Unit Tests

        [Fact]
        public async Task AddEvent_ShouldReturnEventId_WhenEventIsAddedSuccessfully()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IEventRepository>();
            eventRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Event>())).ReturnsAsync(1);

            var usersApiClientMock = new Mock<IUsersApiClient>();
            var loggerMock = new Mock<ILogger<EventService>>();

            var eventService = new EventService(eventRepositoryMock.Object, usersApiClientMock.Object, loggerMock.Object);
            var @event = new Event { Name = "Test Event", Time = DateTime.UtcNow, CreatorId = 1 };

            // Act
            var result = await eventService.AddEvent(@event);

            // Assert
            Assert.Equal(1, result);
            eventRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Event>()), Times.Once);
        }

        [Fact]
        public async Task DeleteEvent_ShouldCallDeleteByIdAsync_WhenIdIsValid()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IEventRepository>();
            eventRepositoryMock.Setup(repo => repo.DeleteByIdAsync(1)).ReturnsAsync(true);

            var usersApiClientMock = new Mock<IUsersApiClient>();
            var loggerMock = new Mock<ILogger<EventService>>();

            var eventService = new EventService(eventRepositoryMock.Object, usersApiClientMock.Object, loggerMock.Object);

            // Act
            await eventService.DeleteEvent(1);

            // Assert
            eventRepositoryMock.Verify(repo => repo.DeleteByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetEvent_ShouldReturnEventDto_WhenEventExists()
        {
            // Arrange
            var eventRepositoryMock = new Mock<IEventRepository>();
            eventRepositoryMock.Setup(repo => repo.GetByIdAsync("Polls,EventParticipants",1 ))
                .ReturnsAsync(new Event { Id = 1, Name = "Test Event", CreatorId = 1 });

            var usersApiClientMock = new Mock<IUsersApiClient>();
            usersApiClientMock.Setup(client => client.ProfileAsync(1))
                .ReturnsAsync(new Profile { Id = 1, UserId = 1 });

            var loggerMock = new Mock<ILogger<EventService>>();

            var eventService = new EventService(eventRepositoryMock.Object, usersApiClientMock.Object, loggerMock.Object);

            // Act
            var result = await eventService.GetEvent(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Event.Id);
            Assert.Equal(1, result.Profile.Id);
        }

        #endregion

        #region ParticipantService Unit Tests

        [Fact]
        public async Task AddParticipantToEvent_ShouldReturnTrue_WhenParticipantIsAddedSuccessfully()
        {
            // Arrange
            var eventParticipantRepositoryMock = new Mock<IEventParticipantRepository>();
            eventParticipantRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<EventParticipant>())).ReturnsAsync(1);

            var participantService = new ParticipantService(eventParticipantRepositoryMock.Object);
            var participant = new EventParticipant { EventId = 1, ProfileId = 1 };

            // Act
            var result = await participantService.AddParticipantToEvent(participant);

            // Assert
            Assert.True(result);
            eventParticipantRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<EventParticipant>()), Times.Once);
        }

        [Fact]
        public async Task RemoveParticipantFromEvent_ShouldCallDeleteByIdAsync_WhenParticipantExists()
        {
            // Arrange
            var eventParticipantRepositoryMock = new Mock<IEventParticipantRepository>();
            eventParticipantRepositoryMock.Setup(repo => repo.DeleteByIdAsync(1, 1)).ReturnsAsync(true);

            var participantService = new ParticipantService(eventParticipantRepositoryMock.Object);
            var participant = new EventParticipant { EventId = 1, ProfileId = 1 };

            // Act
            await participantService.RemoveParticipantFromEvent(participant);

            // Assert
            eventParticipantRepositoryMock.Verify(repo => repo.DeleteByIdAsync(1, 1), Times.Once);
        }

        #endregion

        #region PollService Unit Tests

        [Fact]
        public async Task AddPoll_ShouldReturnPollId_WhenPollIsAddedSuccessfully()
        {
            // Arrange
            var pollRepositoryMock = new Mock<IPollRepository>();
            pollRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Poll>())).ReturnsAsync(1);

            var pollService = new PollService(pollRepositoryMock.Object);
            var poll = new Poll { EventId = 1, ProfileId = 1, PreferredDates = DateTime.UtcNow.ToString() };

            // Act
            var result = await pollService.AddPoll(poll);

            // Assert
            Assert.Equal(1, result);
            pollRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Poll>()), Times.Once);
        }

        [Fact]
        public async Task DeletePoll_ShouldCallDeleteByIdAsync_WhenPollExists()
        {
            // Arrange
            var pollRepositoryMock = new Mock<IPollRepository>();
            pollRepositoryMock.Setup(repo => repo.DeleteByIdAsync(1)).ReturnsAsync(true);

            var pollService = new PollService(pollRepositoryMock.Object);

            // Act
            await pollService.DeletePoll(1);

            // Assert
            pollRepositoryMock.Verify(repo => repo.DeleteByIdAsync(1), Times.Once);
        }



        #endregion

        #region EventController Integration Tests

        [Fact]
        public async Task AddEvent_ShouldReturnOkWithEventId_WhenEventIsAddedSuccessfully()
        {
            // Arrange
            var eventServiceMock = new Mock<IEventService>();
            eventServiceMock.Setup(service => service.AddEvent(It.IsAny<Event>())).ReturnsAsync(1);

            var eventRepositoryMock = new Mock<IEventRepository>();

            var eventController = new EventController(eventServiceMock.Object, eventRepositoryMock.Object);
            var addEventDto = new AddEventDto { Name = "Test Event", Time = DateTime.UtcNow, CreatorId = 1 };

            // Act
            var result = await eventController.AddEvent(addEventDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, okResult.Value);
        }

        [Fact]
        public async Task DeleteEvent_ShouldReturnOk_WhenEventIsDeletedSuccessfully()
        {
            // Arrange
            var eventServiceMock = new Mock<IEventService>();
            eventServiceMock.Setup(service => service.DeleteEvent(1)).Returns(Task.CompletedTask);

            var eventRepositoryMock = new Mock<IEventRepository>();

            var eventController = new EventController(eventServiceMock.Object, eventRepositoryMock.Object);

            // Act
            var result = await eventController.DeleteEvent(1);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetEvent_ShouldReturnOkWithEventDto_WhenEventExists()
        {
            // Arrange
            var eventServiceMock = new Mock<IEventService>();
            eventServiceMock.Setup(service => service.GetEvent(1))
                .ReturnsAsync(new EventDto { Event = new Event { Id = 1 }, Profile = new Profile { Id = 1 } });

            var eventRepositoryMock = new Mock<IEventRepository>();

            var eventController = new EventController(eventServiceMock.Object, eventRepositoryMock.Object);

            // Act
            var result = await eventController.GetEvent(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var eventDto = Assert.IsType<EventDto>(okResult.Value);
            Assert.Equal(1, eventDto.Event.Id);
        }

        #endregion

        #region ParticipantController Integration Tests

        [Fact]
        public async Task AddParticipantToEvent_ShouldReturnOk_WhenParticipantIsAddedSuccessfully()
        {
            // Arrange
            var participantServiceMock = new Mock<IParticipantService>();
            participantServiceMock.Setup(service => service.AddParticipantToEvent(It.IsAny<EventParticipant>())).ReturnsAsync(true);

            var participantController = new ParticipantController(participantServiceMock.Object);
            var participant = new EventParticipant { EventId = 1, ProfileId = 1 };

            // Act
            var result = await participantController.AddParticipantToEvent(participant);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task RemoveParticipantFromEvent_ShouldReturnOk_WhenParticipantIsRemovedSuccessfully()
        {
            // Arrange
            var participantServiceMock = new Mock<IParticipantService>();
            participantServiceMock.Setup(service => service.RemoveParticipantFromEvent(It.IsAny<EventParticipant>())).Returns(Task.CompletedTask);

            var participantController = new ParticipantController(participantServiceMock.Object);

            // Act
            var result = await participantController.RemoveParticipantFromEvent(1, 1);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        #endregion

        #region PollController Integration Tests

        [Fact]
        public async Task AddPoll_ShouldReturnOkWithPollId_WhenPollIsAddedSuccessfully()
        {
            // Arrange
            var pollServiceMock = new Mock<IPollService>();
            pollServiceMock.Setup(service => service.AddPoll(It.IsAny<Poll>())).ReturnsAsync(1);

            var pollRepositoryMock = new Mock<IPollRepository>();

            var pollController = new PollController(pollServiceMock.Object, pollRepositoryMock.Object);
            var addPollDto = new AddPollDto { EventId = 1, ProfileId = 1, PreferredDates = DateTime.UtcNow.ToString() };

            // Act
            var result = await pollController.AddPoll(addPollDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, okResult.Value);
        }

        [Fact]
        public async Task DeletePoll_ShouldReturnOk_WhenPollIsDeletedSuccessfully()
        {
            // Arrange
            var pollServiceMock = new Mock<IPollService>();
            pollServiceMock.Setup(service => service.DeletePoll(1)).Returns(Task.CompletedTask);

            var pollRepositoryMock = new Mock<IPollRepository>();

            var pollController = new PollController(pollServiceMock.Object, pollRepositoryMock.Object);

            // Act
            var result = await pollController.DeletePoll(1);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetPollByProfileId_ShouldReturnOkWithPoll_WhenPollExists()
        {
            // Arrange
            var pollServiceMock = new Mock<IPollService>();
            pollServiceMock.Setup(service => service.GetPollByProfileId(1))
                .ReturnsAsync(new Poll { Id = 1, ProfileId = 1 });

            var pollRepositoryMock = new Mock<IPollRepository>();

            var pollController = new PollController(pollServiceMock.Object, pollRepositoryMock.Object);

            // Act
            var result = await pollController.GetPollByProfileId(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var poll = Assert.IsType<Poll>(okResult.Value);
            Assert.Equal(1, poll.Id);
        }

        #endregion
    }
}
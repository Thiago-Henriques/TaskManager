using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly Mock<ILogger<TasksController>> _loggerMock;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _loggerMock = new Mock<ILogger<TasksController>>();
            _controller = new TasksController(_taskServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithTasks()
        {
            // Arrange
            var tasks = new List<TaskItem> { new TaskItem { Id = Guid.NewGuid(), Title = "Test", Description = "Desc" } };
            _taskServiceMock.Setup(s => s.GetAllTasksAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(tasks, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenTaskExists()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem { Id = taskId, Title = "Test", Description = "Desc" };
            _taskServiceMock.Setup(s => s.GetTaskByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            var result = await _controller.GetById(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(task, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _taskServiceMock.Setup(s => s.GetTaskByIdAsync(taskId)).ReturnsAsync((TaskItem)null);

            // Act
            var result = await _controller.GetById(taskId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenTaskIsValid()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Test", Description = "Desc" };
            _taskServiceMock.Setup(s => s.AddTaskAsync(task)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(task);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(task, createdResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenTaskIsValid()
        {
            // Arrange
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Test", Description = "Desc" };
            _taskServiceMock.Setup(s => s.UpdateTaskAsync(task)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(task.Id, task);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenTaskIsDeleted()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _taskServiceMock.Setup(s => s.DeleteTaskAsync(taskId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(taskId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}

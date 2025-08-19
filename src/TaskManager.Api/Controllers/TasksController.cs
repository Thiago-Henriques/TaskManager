using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all tasks");
                var tasks = await _taskService.GetAllTasksAsync();
                _logger.LogInformation("Retrieved {Count} tasks", tasks.Count());
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all tasks");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving task with ID: {TaskId}", id);
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found", id);
                    return NotFound();
                }
                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving task with ID {TaskId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> Create(TaskItem task)
        {
            try
            {
                _logger.LogInformation("Creating new task with title: {Title}", task.Title);
                await _taskService.AddTaskAsync(task);
                _logger.LogInformation("Successfully created task with ID: {TaskId}", task.Id);
                return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid task data provided");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating task");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TaskUpdateDTO updateDto)
        {
            try
            {
                var existingTask = await _taskService.GetTaskByIdAsync(id);
                if (existingTask == null)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found for update", id);
                    return NotFound();
                }

                UpdateTaskProperties(updateDto, existingTask);

                await _taskService.UpdateTaskAsync(existingTask);
                _logger.LogInformation("Successfully updated task with ID: {TaskId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating task with ID {TaskId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private static void UpdateTaskProperties(TaskUpdateDTO updateDto, TaskItem existingTask)
        {
            existingTask.Title = updateDto.Title;
            existingTask.Description = updateDto.Description;
            existingTask.DueDate = updateDto.DueDate;
            existingTask.Status = updateDto.Status;
            existingTask.UserId = updateDto.UserId;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting task with ID: {TaskId}", id);
                await _taskService.DeleteTaskAsync(id);
                _logger.LogInformation("Successfully deleted task with ID: {TaskId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting task with ID {TaskId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

using Microsoft.Extensions.Logging;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            _logger.LogInformation("Retrieving all tasks from repository");
            var tasks = await _taskRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} tasks from repository", tasks.Count());
            return tasks;
        }

        public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving task with ID: {TaskId}", id);
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found", id);
            }
            return task;
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                _logger.LogWarning("Attempted to add task with empty title");
                throw new ArgumentException("Title is required");
            }

            _logger.LogInformation("Adding new task with title: {Title}", task.Title);
            await _taskRepository.AddAsync(task);
            _logger.LogInformation("Successfully added task with ID: {TaskId}", task.Id);
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            _logger.LogInformation("Updating task with ID: {TaskId}", task.Id);
            await _taskRepository.UpdateAsync(task);
            _logger.LogInformation("Successfully updated task with ID: {TaskId}", task.Id);
        }

        public async Task DeleteTaskAsync(Guid id)
        {
            _logger.LogInformation("Deleting task with ID: {TaskId}", id);
            await _taskRepository.DeleteAsync(id);
            _logger.LogInformation("Successfully deleted task with ID: {TaskId}", id);
        }
    }
}
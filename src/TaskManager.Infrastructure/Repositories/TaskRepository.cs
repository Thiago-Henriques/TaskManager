using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<TaskRepository> _logger;

        public TaskRepository(IConfiguration configuration, ILogger<TaskRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            _logger.LogDebug("Executing GetAllAsync query");
            var tasks = new List<TaskItem>();
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("SELECT * FROM Tasks", connection);
                
                await connection.OpenAsync();
                _logger.LogDebug("Database connection opened successfully");
                
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tasks.Add(new TaskItem
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("Id")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                        Status = (TaskItemStatus)reader.GetInt32(reader.GetOrdinal("Status")),
                        UserId = reader.GetGuid(reader.GetOrdinal("UserId"))
                    });
                }
                
                _logger.LogInformation("Successfully retrieved {Count} tasks from database", tasks.Count);
                return tasks;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while retrieving all tasks");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all tasks");
                throw;
            }
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id)
        {
            _logger.LogDebug("Executing GetByIdAsync query for task ID: {TaskId}", id);
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("SELECT * FROM Tasks WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                
                await connection.OpenAsync();
                _logger.LogDebug("Database connection opened successfully");
                
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var task = new TaskItem
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("Id")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                        Status = (TaskItemStatus)reader.GetInt32(reader.GetOrdinal("Status")),
                        UserId = reader.GetGuid(reader.GetOrdinal("UserId"))
                    };
                    _logger.LogInformation("Successfully retrieved task with ID: {TaskId}", id);
                    return task;
                }
                
                _logger.LogInformation("No task found with ID: {TaskId}", id);
                return null;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while retrieving task with ID: {TaskId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving task with ID: {TaskId}", id);
                throw;
            }
        }

        public async Task AddAsync(TaskItem task)
        {
            _logger.LogDebug("Executing AddAsync query for task with title: {Title}", task.Title);
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(
                    "INSERT INTO Tasks (Id, Title, Description, DueDate, Status, UserId) " +
                    "VALUES (@Id, @Title, @Description, @DueDate, @Status, @UserId)", connection);

                command.Parameters.AddWithValue("@Id", task.Id);
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@DueDate", task.DueDate);
                command.Parameters.AddWithValue("@Status", (int)task.Status);
                command.Parameters.AddWithValue("@UserId", task.UserId);

                await connection.OpenAsync();
                _logger.LogDebug("Database connection opened successfully");
                
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("Successfully added task with ID: {TaskId}", task.Id);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while adding task with ID: {TaskId}", task.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding task with ID: {TaskId}", task.Id);
                throw;
            }
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _logger.LogDebug("Executing UpdateAsync query for task ID: {TaskId}", task.Id);
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(
                    "UPDATE Tasks SET Title = @Title, Description = @Description, " +
                    "DueDate = @DueDate, Status = @Status, UserId = @UserId " +
                    "WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", task.Id);
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@DueDate", task.DueDate);
                command.Parameters.AddWithValue("@Status", (int)task.Status);
                command.Parameters.AddWithValue("@UserId", task.UserId);

                await connection.OpenAsync();
                _logger.LogDebug("Database connection opened successfully");
                
                var rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Successfully updated task with ID: {TaskId}", task.Id);
                }
                else
                {
                    _logger.LogWarning("No task found to update with ID: {TaskId}", task.Id);
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while updating task with ID: {TaskId}", task.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating task with ID: {TaskId}", task.Id);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogDebug("Executing DeleteAsync query for task ID: {TaskId}", id);
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("DELETE FROM Tasks WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                await connection.OpenAsync();
                _logger.LogDebug("Database connection opened successfully");
                
                var rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Successfully deleted task with ID: {TaskId}", id);
                }
                else
                {
                    _logger.LogWarning("No task found to delete with ID: {TaskId}", id);
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while deleting task with ID: {TaskId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting task with ID: {TaskId}", id);
                throw;
            }
        }
    }
}

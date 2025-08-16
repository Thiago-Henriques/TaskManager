using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            var tasks = new List<TaskItem>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT * FROM Tasks", connection);
            await connection.OpenAsync();
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

            return tasks;
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT * FROM Tasks WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new TaskItem
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                    Status = (TaskItemStatus)reader.GetInt32(reader.GetOrdinal("Status")),
                    UserId = reader.GetGuid(reader.GetOrdinal("UserId"))
                };
            }
            return null;
        }

        public async Task AddAsync(TaskItem task)
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
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "UPDATE Tasks SET Title=@Title, Description=@Description, DueDate=@DueDate, Status=@Status, UserId=@UserId WHERE Id=@Id",
                connection);

            command.Parameters.AddWithValue("@Id", task.Id);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@DueDate", task.DueDate);
            command.Parameters.AddWithValue("@Status", (int)task.Status);
            command.Parameters.AddWithValue("@UserId", task.UserId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("DELETE FROM Tasks WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            _logger.LogDebug("Executing GetByIdAsync query for user ID: {UserId}", id);
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("SELECT * FROM Users WHERE Id=@Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                
                await connection.OpenAsync();
                _logger.LogDebug("Database connection opened successfully");
                
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var user = new User
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                    };
                    _logger.LogInformation("Successfully retrieved user with ID: {UserId}", id);
                    return user;
                }
                
                _logger.LogInformation("No user found with ID: {UserId}", id);
                return null;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while retrieving user with ID: {UserId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving user with ID: {UserId}", id);
                throw;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            _logger.LogDebug("Executing GetByEmailAsync query for email: {Email}", email);
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("SELECT * FROM Users WHERE Email=@Email", connection);
                command.Parameters.AddWithValue("@Email", email);
                
                await connection.OpenAsync();
                _logger.LogDebug("Database connection opened successfully");
                
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var user = new User
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                    };
                    _logger.LogInformation("Successfully retrieved user with email: {Email}", email);
                    return user;
                }
                
                _logger.LogInformation("No user found with email: {Email}", email);
                return null;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while retrieving user with email: {Email}", email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving user with email: {Email}", email);
                throw;
            }
        }

        public async Task AddAsync(User user)
        {
            _logger.LogDebug("Executing AddAsync query for user with email: {Email}", user.Email);
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(
                    "INSERT INTO Users (Id, Name, Email, PasswordHash) VALUES (@Id, @Name, @Email, @PasswordHash)",
                    connection);

                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                await connection.OpenAsync();
                _logger.LogDebug("Database connection opened successfully");
                
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("Successfully added user with ID: {UserId} and email: {Email}", user.Id, user.Email);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while adding user with email: {Email}", user.Email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding user with email: {Email}", user.Email);
                throw;
            }
        }
    }
}

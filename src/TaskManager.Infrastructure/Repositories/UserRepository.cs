using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT * FROM Users WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                };
            }
            return null;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT * FROM Users WHERE Email=@Email", connection);
            command.Parameters.AddWithValue("@Email", email);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                };
            }
            return null;
        }

        public async Task AddAsync(User user)
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
            await command.ExecuteNonQueryAsync();
        }
    }
}

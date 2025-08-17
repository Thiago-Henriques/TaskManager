using Microsoft.Extensions.Logging;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            _logger.LogInformation("Retrieving user with email: {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email);
            
            if (user == null)
            {
                _logger.LogWarning("User not found with email: {Email}", email);
            }
            else
            {
                _logger.LogInformation("Successfully retrieved user with email: {Email}", email);
            }
            
            return user;
        }

        public async Task AddUserAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                _logger.LogWarning("Attempted to add user with empty email");
                throw new ArgumentException("Email is required");
            }

            _logger.LogInformation("Adding new user with email: {Email}", user.Email);
            await _userRepository.AddAsync(user);
            _logger.LogInformation("Successfully added user with ID: {UserId}", user.Id);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving user with ID: {UserId}", id);
            var user = await _userRepository.GetByIdAsync(id);
            
            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id);
            }
            else
            {
                _logger.LogInformation("Successfully retrieved user with ID: {UserId}", id);
            }
            
            return user;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            _logger.LogInformation("Attempting login for user with email: {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email);
            
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found with email: {Email}", email);
                return null;
            }

            if (user.PasswordHash == password)
            {
                _logger.LogInformation("User successfully logged in with email: {Email}", email);
                return user;
            }

            _logger.LogWarning("Login failed - invalid password for user with email: {Email}", email);
            return null;
        }
    }
}

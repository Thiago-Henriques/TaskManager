using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task<User?> LoginAsync(string email, string password);
    }
}

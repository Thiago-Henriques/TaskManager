using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            // --- Repositories ---
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // --- Application Services ---
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            #region Repositories
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            #endregion

            #region Application Services
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IUserService, UserService>();
            #endregion

            #region Logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            #endregion

            return services;
        }
    }
}

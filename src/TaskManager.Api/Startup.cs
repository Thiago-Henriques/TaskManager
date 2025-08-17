using Microsoft.OpenApi.Models;
using TaskManager.Infrastructure.Configuration;
using Serilog;
using Serilog.Events;

namespace TaskManager.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "logs/taskmanager-.log",
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    retainedFileCountLimit: 10,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        // Configure Dependency Injection
        public void ConfigureServices(IServiceCollection services)
        {
            #region Logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });
            #endregion

            #region Controllers
            services.AddControllers();
            #endregion

            #region Infrastructure & Application DI
            services.AddInfrastructureDependencies(); 
            #endregion
        }

        // Configure Middleware Pipeline
        public void Configure(WebApplication app)
        {
            #region Development Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API V1");
                    c.RoutePrefix = string.Empty;
                });
            }
            #endregion

            #region Common Middleware
            app.UseHttpsRedirection();
            app.MapControllers();
            #endregion
        }
    }
}

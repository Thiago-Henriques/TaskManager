using Microsoft.OpenApi.Models;
using TaskManager.Infrastructure.Configuration;

namespace TaskManager.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Configure Dependency Injection
        public void ConfigureServices(IServiceCollection services)
        {
            #region Controllers
            services.AddControllers();
            #endregion

            #region Infrastructure & Application DI
            services.AddInfrastructureDependencies(); 
            #endregion

            #region Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TaskManager API",
                    Version = "v1",
                    Description = "API for managing tasks and users"
                });
            });
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
            app.UseAuthorization();
            app.MapControllers();
            #endregion
        }
    }
}

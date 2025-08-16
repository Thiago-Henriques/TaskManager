using Microsoft.OpenApi.Models;
using TaskManager.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

#region Services Configuration
// Add controllers
builder.Services.AddControllers();

// Add infrastructure dependencies (repositories + services)
builder.Services.AddInfrastructureDependencies();
#endregion

#region Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskManager API",
        Version = "v1",
        Description = "API for managing tasks and users"
    });
});
#endregion

var app = builder.Build();

#region Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API V1");
        c.RoutePrefix = string.Empty;
    });
}

// HTTPS redirection
app.UseHttpsRedirection();

// Authorization middleware
app.UseAuthorization();

// Map controllers
app.MapControllers();
#endregion

app.Run();

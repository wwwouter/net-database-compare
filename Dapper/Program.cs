using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configuration for the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register the DatabaseConnectionFactory as a singleton to be used by repositories
builder.Services.AddSingleton(new DatabaseConnectionFactory(connectionString));

// Register your Dapper-based repositories as scoped or transient as needed
// Assuming there is a repository interface IEmployeeProjectRepository and its implementation EmployeeProjectRepository
builder.Services.AddScoped<IEmployeeProjectRepository, EmployeeProjectRepository>();

// Register the custom migration service that uses Dapper to apply migrations
builder.Services.AddScoped<DapperMigrationService>();

var app = builder.Build();

// Apply migrations using the custom migration service
using (var scope = app.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<DapperMigrationService>();
    try
    {
        await migrationService.ApplyMigrationsAsync();
    }
    catch (Exception ex)
    {
        // Log the exception and possibly stop the application
        var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations.");

        // Depending on your application's needs, you might want to stop the application startup process here
        Environment.Exit(1);
    }
}

// Add services to the container.
builder.Services.AddControllersWithViews();

// Further configuration goes here

app.Run();

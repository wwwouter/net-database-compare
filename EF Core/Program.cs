var builder = WebApplication.CreateBuilder(args);

// Other configurations...

var connectionString = builder.Configuration.GetConnectionString("YourConnectionStringName");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));

// Read the migration scripts path from configuration
var migrationScriptsPath = builder.Configuration.GetValue<string>("MigrationSettings:ScriptsPath");

// Register the MigrationService with the DI container
builder.Services.AddScoped<MigrationService>(provider =>
    new MigrationService(
        provider.GetRequiredService<AppDbContext>(),
        migrationScriptsPath
    ));

// Continue setting up the application...
var app = builder.Build();


// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<MigrationService>();
    try
    {
        await migrationService.ApplyMigrations();
    }
    catch (Exception ex)
    {
        // Log the exception, perform any necessary cleanup, and stop application startup
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations. Application will be stopped.");

        // Fail fast by exiting the application
        Environment.Exit(1);
    }
}

// Further app configuration...

app.Run();

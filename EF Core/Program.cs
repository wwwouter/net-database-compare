var builder = WebApplication.CreateBuilder(args);

// Other configurations...

var connectionString = builder.Configuration.GetConnectionString("YourConnectionStringName");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));

// Continue setting up the application...
var app = builder.Build();

// Further app configuration...

app.Run();

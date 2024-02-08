using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.NetTopologySuite;
using System;

var builder = WebApplication.CreateBuilder(args);

// Other configurations...

// Configure NHibernate session factory
var sessionFactory = Fluently.Configure()
    .Database(MsSqlConfiguration.MsSql2012
        .ConnectionString(builder.Configuration.GetConnectionString("YourConnectionStringName"))
        .ShowSql()
        .Dialect<MsSql2012Dialect>()
        .Driver<SqlClientDriver>()
        .UseNetTopologySuite()
    )
    .Mappings(m => m.FluentMappings
        .AddFromAssemblyOf<Employee>()) // Assuming your mappings are set up with Fluent NHibernate
    .BuildSessionFactory();

// Register NHibernate ISession as scoped dependency
builder.Services.AddScoped<ISession>(provider => sessionFactory.OpenSession());

// Register your services and repositories as needed
builder.Services.AddScoped<IEmployeeProjectRepository, EmployeeProjectRepository>();
builder.Services.AddScoped<ITransactionService, NhTransactionService>();

var app = builder.Build();

// Apply custom migrations or schema updates
using (var scope = app.Services.CreateScope())
{
    var session = scope.ServiceProvider.GetRequiredService<ISession>();
    // Perform your schema validation or migration logic here
    // Note: NHibernate does not have built-in support for migrations like EF Core. You might need to use SQL scripts or a third-party library like FluentMigrator.
}

// Continue setting up the application...

app.Run();

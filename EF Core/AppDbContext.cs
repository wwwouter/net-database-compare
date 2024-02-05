using Microsoft.EntityFrameworkCore;
using System;

public class AppDbContext : DbContext
{
    // Other DbSet definitions

    public DbSet<ProjectSummaryDto> ProjectSummaries { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // If necessary, you can provide further configuration here. For example:
        modelBuilder.Entity<ProjectSummaryDto>().HasNoKey().ToView("ProjectSummaries");
        // .HasNoKey() indicates that this entity does not have a primary key, which is common for views.
        // .ToView("ProjectSummaries") specifies the name of the view in the database.

        // Further configurations...
    }
}

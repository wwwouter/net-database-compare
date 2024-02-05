using Microsoft.EntityFrameworkCore;
using System;

public class AppDbContext : DbContext
{
    // DbSet definitions for your entities
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ProjectCustomer> ProjectCustomers { get; set; }
    public DbSet<EmployeeHierarchy> EmployeeHierarchies { get; set; }
    public DbSet<ProjectSummaryDto> ProjectSummaries { get; set; } // For read-only view

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuring ProjectSummaryDto to map to a database view
        modelBuilder.Entity<ProjectSummaryDto>()
            .HasNoKey()
            .ToView("ProjectSummaries");

        // Example configurations for other entities

        // Employee entity configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Department).IsRequired().HasMaxLength(20);
            // Configure one-to-many or many-to-many relationships here
            // Example for one-to-many: Employee to Projects
            entity.HasMany(e => e.AssignedProjects)
                  .WithOne(p => p.Employee)
                  .HasForeignKey(p => p.EmployeeAssigned);
        });

        // Project entity configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Budget).HasColumnType("decimal(19, 4)");
            // If using enums and want to store them as string
            entity.Property(e => e.Status)
                  .HasConversion<string>();
            // Relationships are configured in the Employee entity
        });

        // Customer entity configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(254);
            // JSON column configuration, if using SQL Server or a database that supports JSON columns
            entity.Property(c => c.JSONData).HasColumnType("nvarchar(max)");
        });

    }
}


I'm writing a repository with EF Core to compare different dataaccess packages. Write next missing method. Keep DTO nullable values nullable. If not clear, just assume a query, based on the description. MS SQL Server is the database.


Can you create the necessary files to Define the Entity Classes (in full) for this schema for ef core 8 and .NET 8 (used with an Entity Framework Core DbContext)? Add Data Annotations if relevant.
Use the correct type for Customers.GeographicLocation.  Projects.Status and Projects.Priority should be enums.


Step 1: 
Step 2: Configure DbContext
Step 3: Implement Repository



Let SQL Server Handle It: You can configure Entity Framework Core to let SQL Server handle the default value by not including the CreatedOn field in your EF Core insert statement. This can be achieved by configuring the CreatedOn property as database-generated in your EF Core model configuration, typically in the OnModelCreating method of your DbContext:
``` csharp
modelBuilder.Entity<Employee>()
    .Property(e => e.CreatedOn)
    .HasDefaultValueSql("SYSDATETIME()")
    .ValueGeneratedOnAdd();
```




I'm writing a repository with EF Core to compare different dataaccess packages. Keep DTO nullable values nullable. If not clear, just assume a query, based on the description. MS SQL Server is the database.
I want to showcase working with transactions. Implement the following methods in the repository: RunTwoUpdatesInSingleTransaction, Operation1InATransaction and Operation2InATransaction.

I 
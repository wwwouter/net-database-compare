

- Implement Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomersWithFavoriteNumber(int favoriteNumber); in ef core
- dapper
    - https://github.com/DapperLib/Dapper.Contrib
        - T Get<T>(id);
        - IEnumerable<T> GetAll<T>();
        - int Insert<T>(T obj);
        - int Insert<T>(Enumerable<T> list);
        - bool Update<T>(T obj);
            - with updatedOn
        - bool Update<T>(Enumerable<T> list);
        - bool Delete<T>(T obj);
        - bool Delete<T>(Enumerable<T> list);
        - bool DeleteAll<T>(); 
        - T Get<T,W>(string table, W where);       
            - in repo wrap in a method with table set.

I'm writing a repository with EF Core to compare different data access packages. Write next missing method. Keep DTO nullable values nullable. If not clear, just assume a query, based on the description. MS SQL Server is the database.


Can you create the necessary files to Define the Entity Classes (in full) for this schema for ef core 8 and .NET 8 (used with an Entity Framework Core DbContext)? Add Data Annotations if relevant.
Use the correct type for Customers.GeographicLocation.  Projects.Status and Projects.Priority should be enums.


Step 1: 
Step 2: Configure DbContext
Step 3: Implement Repository
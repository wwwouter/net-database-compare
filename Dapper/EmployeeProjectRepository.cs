using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

public class EmployeeProjectRepository : IEmployeeProjectRepository
{
    private readonly string _connectionString;

    public EmployeeProjectRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    private async Task ExecuteAsync(string sql, object param = null)
    {
        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, param);
        }
    }

    private async Task<List<T>> QueryAsync<T>(string sql, object param = null)
    {
        using (var connection = CreateConnection())
        {
            var results = await connection.QueryAsync<T>(sql, param);
            return results.ToList();
        }
    }

    public async Task AddEmployee(EmployeeAddDto employee)
    {
        var sql = @"
INSERT INTO Employees (Id, Name, Age, Department, HireDate, Salary, AddressLine1, AddressLine2, City, CreatedOn, UpdatedOn) 
VALUES (@EmployeeID, @Name, @Age, @Department, @HireDate, @Salary, @AddressLine1, @AddressLine2, @City, @CreatedOn, @UpdatedOn)";


        await ExecuteAsync(sql, new
        {
            employee.EmployeeID,
            employee.Name,
            employee.Age,
            employee.Department,
            employee.HireDate,
            employee.Salary,
            employee.AddressLine1,
            employee.AddressLine2,
            employee.City,
            employee.CreatedOn,
            employee.UpdatedOn
        });

    }
    public async Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate)
    {
        var sql = @"
UPDATE Employees 
SET Name = @Name 
WHERE Id = @EmployeeID";

        await ExecuteAsync(sql, new
        {
            employeeUpdate.EmployeeID,
            employeeUpdate.Name
        });
    }

    public async Task DeleteEmployeeById(EmployeeDeleteDto employeeDelete)
    {
        var sql = @"
DELETE FROM Employees 
WHERE Id = @EmployeeID";

        await ExecuteAsync(sql, new
        {
            employeeDelete.EmployeeID
        });
    }


    public async Task<List<GetEmployeesByCityDto>> GetEmployeesByCity(EmployeeCityQueryDto cityQuery)
    {
        var sql = @"
SELECT Id as EmployeeID, Name, City 
FROM Employees 
WHERE City = @City";

        return await QueryAsync<GetEmployeesByCityDto>(sql, new { cityQuery.City });
    }

    public async Task<List<ProjectDto>> GetProjectsByEmployeeId(EmployeeProjectsQueryDto employeeProjectsQuery)
    {
        var sql = @"
SELECT p.Id as ProjectID, p.Name, p.StartDate, p.EndDate, p.Budget, p.Status, p.LogoSvg, p.Notes, p.Progress, p.Priority, p.EmployeeAssigned
FROM Projects p
INNER JOIN EmployeeProjects ep ON p.Id = ep.ProjectId
WHERE ep.EmployeeId = @EmployeeID";

        return await QueryAsync<ProjectDto>(sql, new { employeeProjectsQuery.EmployeeID });
    }

    public async Task<List<ProjectDto>> GetProjectsByCustomerId(CustomerProjectsQueryDto customerProjectsQuery)
    {
        var sql = @"
SELECT p.Id as ProjectID, p.Name, p.StartDate, p.EndDate, p.Budget, p.Status, p.LogoSvg, p.Notes, p.Progress, p.Priority, p.EmployeeAssigned
FROM Projects p
WHERE p.CustomerId = @CustomerID";

        return await QueryAsync<ProjectDto>(sql, new { customerProjectsQuery.CustomerID });
    }

    public async Task<List<EmployeeDto>> FullTextSearch(FullTextSearchDto searchQuery)
    {
        // Assuming full-text search is set up on the Employees table, for columns like Name, Department, etc.
        var sql = @"
SELECT Id as EmployeeID, Name, Age, Department, HireDate, Salary, AddressLine1, AddressLine2, City 
FROM Employees 
WHERE CONTAINS((Name, Department, City), @SearchTerm)";

        // The @SearchTerm parameter should be formatted appropriately for full-text search.
        // For a simple search, you can use the search term directly. For more complex searches, you might need to format the search term (e.g., prefixing with '*', using BOOLEAN mode, etc.)
        var formattedSearchTerm = $"\"*{searchQuery.SearchTerm}*\""; // Simple formatting example. Adjust based on your needs and SQL Server's full-text search syntax.

        return await QueryAsync<EmployeeDto>(sql, new { SearchTerm = formattedSearchTerm });
    }

    public async Task<List<EmployeeProjectOuterJoinDto>> GetEmployeeProjectsWithOuterJoin()
    {
        var sql = @"
SELECT e.Id as EmployeeID, p.Id as ProjectID
FROM Employees e
LEFT OUTER JOIN EmployeeProjects ep ON e.Id = ep.EmployeeId
LEFT OUTER JOIN Projects p ON ep.ProjectId = p.Id";

        return await QueryAsync<EmployeeProjectOuterJoinDto>(sql);
    }



}

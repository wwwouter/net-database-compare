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

    public async Task<List<EmployeeSubqueryDto>> GetEmployeesWithSubquery()
    {
        // Define a threshold for project budgets
        var budgetThreshold = 100000M;

        var sql = @"
SELECT e.Id AS EmployeeID, e.Name
FROM Employees e
WHERE EXISTS (
    SELECT 1
    FROM Projects p
    WHERE p.EmployeeAssigned = e.Id AND p.Budget > @BudgetThreshold
)";

        return await QueryAsync<EmployeeSubqueryDto>(sql, new { BudgetThreshold = budgetThreshold });
    }


    public async Task EditJsonData(EditJsonDataDto editJsonDataDto)
    {
        var sql = @"
UPDATE YourTableName 
SET JSONData = @UpdatedJson 
WHERE Id = @EntityId";

        // Assuming the JsonDataDto's content is serialized to a JSON string for the update.
        // This serialization step depends on how your data is structured and may need adjustment.
        var updatedJson = JsonConvert.SerializeObject(editJsonDataDto.UpdatedJsonData);

        await ExecuteAsync(sql, new
        {
            EntityId = editJsonDataDto.EntityId,
            UpdatedJson = updatedJson
        });
    }

    public async Task AppendNumberToJsonData(AppendNumberToJsonDataDto appendNumberDto)
    {
        var sql = @"
UPDATE YourTableName
SET JSONData = JSON_MODIFY(JSONData, 'append $.FavoriteNumbers', @NumberToAppend)
WHERE Id = @EntityId";

        await ExecuteAsync(sql, new
        {
            EntityId = appendNumberDto.EntityId,
            NumberToAppend = appendNumberDto.NumberToAppend
        });
    }

    public async Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomerBasedOnJsonProperty(JsonPropertyQueryDto jsonPropertyQuery)
    {

        var allowedPropertyNames = new HashSet<string> { "Name", "Category" };

        // Validate user input against the whitelist
        if (!allowedPropertyNames.Contains(jsonPropertyQuery.JsonPropertyName))
        {
            throw new ArgumentException("Invalid JSON property name.");
        }

        var jsonPath = "$." + jsonPropertyQuery.JsonPropertyName;
        var sql = $@"
SELECT Id as CustomerID, Name, Age, Email, PhoneNumber, AddressLine1, AddressLine2, City, Country, GeographicLocation, LoyaltyPoints, LastPurchaseDate, Notes, JSONData
FROM Customers
WHERE JSON_VALUE(JSONData, @JsonPath) = @Value";


        return QueryAsync<CustomerBasedOnJsonPropertyDto>(sql, new
        {
            JsonPath = jsonPath, // JSON path is directly injected into SQL, ensure it's safe
            Value = jsonPropertyQuery.Value
        });

    }



    public async Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomersWithFavoriteNumber(int favoriteNumber)
    {
        var sql = @"
SELECT c.Id as CustomerID, c.Name, c.Age, c.Email, c.PhoneNumber, c.AddressLine1, c.AddressLine2, c.City, c.Country, c.GeographicLocation, c.LoyaltyPoints, c.LastPurchaseDate, c.Notes, c.JSONData
FROM Customers c
CROSS APPLY OPENJSON(c.JSONData, '$.FavoriteNumbers')
    WITH (Number int '$') AS favNumbers
WHERE favNumbers.Number = @FavoriteNumber";


        return await QueryAsync<CustomerBasedOnJsonPropertyDto>(sql, new { FavoriteNumber = favoriteNumber });
    }

    public async Task<List<EmployeeHierarchyDto>> GetEmployeeHierarchy(EmployeeHierarchyQueryDto hierarchyQuery)
    {
        var sql = @"
;WITH EmployeeCTE AS (
    SELECT e.Id, e.ManagerId, e.Name
    FROM Employees e
    WHERE e.Id = @EmployeeID
    UNION ALL
    SELECT e.Id, e.ManagerId, e.Name
    FROM Employees e
    INNER JOIN EmployeeCTE ecte ON e.Id = ecte.ManagerId
)
SELECT Id as EmployeeId, ManagerId, Name as EmployeeName
FROM EmployeeCTE";

        return await connection.QueryAsync<EmployeeHierarchyDto>(sql, new { EmployeeID = hierarchyQuery.EmployeeID });
    }


}

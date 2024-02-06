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

    public async Task AddEmployeeWithPartialData(EmployeePartialAddDto employeePartial)
    {
        var sql = @"
INSERT INTO Employees (Id, Name, Age, Department, HireDate, CreatedOn, UpdatedOn, IsActive) 
VALUES (@EmployeeID, @Name, @Age, 'Not Specified', @HireDate, SYSDATETIME(), SYSDATETIME(), 1)";

        await ExecuteAsync(sql, new
        {
            EmployeeID = Guid.NewGuid(), // Assuming the ID is generated here rather than by the client.
            employeePartial.Name,
            employeePartial.Age,
            HireDate = DateTime.UtcNow // Assuming the hire date is set to the current date for demonstration.
                                       // 'Department' is set to 'Not Specified' as a placeholder. Adjust this based on your requirements.
        });
    }


    public async Task RunTwoUpdatesInSingleTransaction(SingleOperationTransactionDto data)
    {
        var updateSql = @"
UPDATE Employees 
SET Name = @Name 
WHERE Id = @EmployeeID";

        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();
            using (var transaction = connection.BeginTransaction())
            {
                // First update operation
                await connection.ExecuteAsync(updateSql, new { EmployeeID = data.id1, Name = data.name1 }, transaction: transaction);

                // Second update operation
                await connection.ExecuteAsync(updateSql, new { EmployeeID = data.id2, Name = data.name2 }, transaction: transaction);

                // Commit transaction
                transaction.Commit();
            }
        }
    }

    public async Task Operation1InATransaction(Guid id, string name)
    {
        var sql = "UPDATE Employees SET Name = @Name WHERE Id = @Id";
        await ExecuteAsync(sql, new { Id = id, Name = name });
    }

    public async Task Operation2InATransaction(Guid id, string name)
    {
        var sql = "UPDATE Employees SET Name = @Name WHERE Id = @Id";
        await ExecuteAsync(sql, new { Id = id, Name = name });
    }

    public async Task BulkInsertEmployees(IEnumerable<EmployeeBulkInsertDto> employeeDtos)
    {
        // or very large datasets, consider using a library like Dapper.Contrib or Dapper Plus, or use the SQL Server specific SqlBulkCopy class for maximum performance.
        var sql = @"INSERT INTO Employees (Id, Name, Age, Department, HireDate, Salary, AddressLine1, AddressLine2, City) VALUES ";

        var parameters = new List<object>();
        var valuesList = new List<string>();
        int count = 0;

        foreach (var employee in employeeDtos)
        {
            count++;
            valuesList.Add($"(@Id{count}, @Name{count}, @Age{count}, @Department{count}, @HireDate{count}, @Salary{count}, @AddressLine1{count}, @AddressLine2{count}, @City{count})");

            parameters.Add(new
            {
                Id = employee.EmployeeID,
                Name = employee.Name,
                Age = employee.Age,
                Department = employee.Department,
                HireDate = employee.HireDate,
                Salary = employee.Salary,
                AddressLine1 = employee.AddressLine1,
                AddressLine2 = employee.AddressLine2,
                City = employee.City
            });
        }

        sql += string.Join(", ", valuesList);

        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, parameters.ToArray());
        }
    }


    public async Task BulkUpdateEmployees(IEnumerable<EmployeeBulkUpdateDto> employeeDtos)
    {
        //  For more substantial performance improvements, especially with very large datasets, 
        //  consider using a library that supports efficient bulk operations with SQL Server, such as 
        //  Dapper Plus, or leveraging SqlBulkCopy with a temporary table and then performing 
        //  a bulk update from the temporary table to the target table.
        var sql = @"UPDATE Employees SET Name = @Name WHERE Id = @EmployeeID";

        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();
            using (var transaction = connection.BeginTransaction())
            {
                foreach (var employee in employeeDtos)
                {
                    await connection.ExecuteAsync(sql, new { EmployeeID = employee.EmployeeID, Name = employee.Name }, transaction: transaction);
                }

                transaction.Commit();
            }
        }
    }

    public async Task<List<EmployeesWithDynamicQueryDto>> GetEmployeesWithDynamicQuery(DynamicQueryDto query)
    {
        var sqlBuilder = new SqlBuilder();
        var template = sqlBuilder.AddTemplate(@"
SELECT Id AS EmployeeID, Name
/**where**/
/**orderby**/
FROM Employees
");

        // Dynamically applying filters
        if (query.Filters != null)
        {
            foreach (var filter in query.Filters)
            {
                var columnName = filter.Key;
                var columnValue = filter.Value;
                if (columnValue != null)
                {
                    // Ensure to handle potential SQL injection by using parameters and not injecting values directly
                    sqlBuilder.Where($"{columnName} LIKE @Value", new { Value = $"%{columnValue}%" });
                }
            }
        }

        // Applying dynamic sorting
        if (query.SortOrder != null && query.SortOrder.Count > 0)
        {
            foreach (var sort in query.SortOrder)
            {
                var direction = sort.Value ? "ASC" : "DESC";
                sqlBuilder.OrderBy($"{sort.Key} {direction}");
            }
        }
        else
        {
            // Default sorting
            sqlBuilder.OrderBy("Name ASC");
        }

        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();
            var results = await connection.QueryAsync<EmployeesWithDynamicQueryDto>(template.RawSql, template.Parameters);
            return results.ToList();
        }
    }

    public async Task<List<EmployeesWithDynamicQueryDto>> GetEmployeesWithDynamicQuery(DynamicQueryDto query)
    {
        var allowedColumns = new HashSet<string> { "Name", "Department", "Age", "City" }; // Define allowed column names

        // Validate column names in filters
        foreach (var filter in query.Filters.Keys)
        {
            if (!allowedColumns.Contains(filter))
            {
                throw new ArgumentException($"Invalid column name in filters: {filter}");
            }
        }

        // Validate column names in sort orders
        foreach (var sort in query.SortOrder.Keys)
        {
            if (!allowedColumns.Contains(sort))
            {
                throw new ArgumentException($"Invalid column name in sort order: {sort}");
            }
        }

        var sqlBuilder = new SqlBuilder();
        var template = sqlBuilder.AddTemplate(@"
SELECT Id AS EmployeeID, Name
/**where**/
/**orderby**/
FROM Employees
");

        // Dynamically applying filters
        foreach (var filter in query.Filters)
        {
            var columnName = filter.Key;
            var columnValue = filter.Value;
            sqlBuilder.Where($"{columnName} LIKE @Value", new { Value = $"%{columnValue}%" });
        }

        // Applying dynamic sorting
        foreach (var sort in query.SortOrder)
        {
            var columnName = sort.Key;
            var direction = sort.Value ? "ASC" : "DESC";
            sqlBuilder.OrderBy($"{columnName} {direction}");
        }

        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();
            var results = await connection.QueryAsync<EmployeesWithDynamicQueryDto>(template.RawSql, template.Parameters);
            return results.ToList();
        }
    }


    public async Task<PagedResultDto<EmployeeDto>> GetEmployeesPagedAndSorted(PagingAndSortingQueryDto query)
    {
        var allowedSortColumns = new HashSet<string> { "Name", "Department", "Age", "City" }; // Define allowed sort columns

        // Validate the SortBy column
        if (!allowedSortColumns.Contains(query.SortBy))
        {
            throw new ArgumentException($"Invalid sort column: {query.SortBy}");
        }

        var sortDirection = query.Ascending ? "ASC" : "DESC";

        // Building the SQL for paging and sorting
        var sql = $@"
SELECT Id, Name, Age, Department, HireDate, Salary, AddressLine1, AddressLine2, City
FROM Employees
ORDER BY {query.SortBy} {sortDirection}
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

SELECT COUNT(*) FROM Employees;";

        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();

            // Using Dapper's QueryMultipleAsync to execute both queries in a single round trip
            using (var multi = await connection.QueryMultipleAsync(sql, new { Offset = (query.PageNumber - 1) * query.PageSize, PageSize = query.PageSize }))
            {
                var employees = (await multi.ReadAsync<EmployeeDto>()).ToList();
                var totalCount = await multi.ReadFirstAsync<int>();

                return new PagedResultDto<EmployeeDto>(employees, totalCount);
            }
        }
    }

    public async Task<List<EmployeeSelfJoinDto>> GetEmployeeManagers()
    {
        // SQL query that performs a self-join on the Employees table
        // to find the manager for each employee
        var sql = @"
SELECT e.Id AS EmployeeID, 
       m.Id AS ManagerID, 
       e.Name AS EmployeeName, 
       m.Name AS ManagerName
FROM Employees e
LEFT JOIN Employees m ON e.ManagerId = m.Id";

        return await connection.QueryAsync<EmployeeSelfJoinDto>(sql);

    }

    public async Task<decimal> GetTotalBudgetForProjects()
    {
        // SQL query to calculate the total budget by summing the Budget column of all projects
        var sql = @"
SELECT SUM(Budget) 
FROM Projects";

        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();

            // Execute the query using Dapper. Use QuerySingleOrDefaultAsync to expect a single return value or default if none
            var totalBudget = await connection.QuerySingleOrDefaultAsync<decimal?>(sql);

            // Return 0 if the result is null (e.g., no projects in the table), otherwise return the total budget
            return totalBudget ?? 0;
        }
    }

    public async Task<List<ProjectSummaryDto>> GetProjectSummaries()
    {
        var sql = @"
SELECT 
    ProjectID,
    Name,
    TotalBudget,
    Status,
    StartDate,
    EndDate,
    Progress,
    Priority,
    EmployeeAssignedName,
    NumberOfCustomers
FROM ProjectSummaries;";

        return await connection.QueryAsync<ProjectSummaryDto>(sql);
    }

    public async Task<List<EmployeeDto>> CallStoredProcedure(StoredProcedureQueryDto query)
    {
        // Define the stored procedure name
        var procedureName = "GetEmployeesByDepartment";

        // Create a new connection
        using (var connection = CreateConnection())
        {
            await connection.OpenAsync();

            // Define the parameters for the stored procedure
            var parameters = new DynamicParameters();
            parameters.Add("@Department", query.Department, DbType.String, ParameterDirection.Input);

            // Execute the stored procedure and map the results to a list of EmployeeDto
            var employees = await connection.QueryAsync<EmployeeDto>(procedureName, parameters, commandType: CommandType.StoredProcedure);

            return employees.ToList();
        }
    }

    public async Task<List<CustomerSpatialQueryDto>> GetCustomersNearLocation(SpatialQueryDto query)
    {
        // Define the SQL query to select customers near the specified location
        var sql = @"
DECLARE @queryPoint geography = geography::Point(@Latitude, @Longitude, 4326);
SELECT 
    Id as CustomerID, 
    Name, 
    Email, 
    PhoneNumber, 
    AddressLine1, 
    AddressLine2, 
    City, 
    Country, 
    GeographicLocation.ToString() as GeographicLocation, 
    LoyaltyPoints, 
    LastPurchaseDate, 
    Notes 
FROM 
    Customers 
WHERE 
    GeographicLocation.STDistance(@queryPoint) <= @Distance";

        return await connection.QueryAsync<CustomerSpatialQueryDto>(sql, new
        {
            query.Latitude,
            query.Longitude,
            query.Distance
        });


    }


}

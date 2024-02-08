using RepoDb;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

public class EmployeeProjectRepository : IEmployeeProjectRepository
{
    private readonly string _connectionString;

    public EmployeeProjectRepository(string connectionString)
    {
        _connectionString = connectionString;
        SqlServerBootstrap.Initialize();
    }
    // Helper method to create and open a SQL connection
    private async Task<SqlConnection> CreateOpenConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    // Generic method to execute a SQL query asynchronously and return a list of DTOs
    public async Task<List<T>> ExecuteQueryAsync<T>(string sql, object param = null) where T : class
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            // Open the connection asynchronously
            await connection.OpenAsync();

            // Execute the query and return the results
            var result = await connection.ExecuteQueryAsync<T>(sql, param);
            return result.ToList();
        }
    }

    // Generic method to query asynchronously based on a condition and return a list of entities
    public async Task<List<T>> QueryAsync<T>(object whereExpression) where T : class
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            // Open the connection asynchronously
            await connection.OpenAsync();

            // Perform the query based on the whereExpression and return the results
            var result = await connection.QueryAsync<T>(whereExpression);
            return result.ToList();
        }
    }


    public async Task AddEmployee(EmployeeAddDto employeeDto)
    {
        var employee = new Employee
        {
            Id = employeeDto.EmployeeID,
            Name = employeeDto.Name,
            Age = employeeDto.Age,
            Department = employeeDto.Department,
            HireDate = employeeDto.HireDate,
            Salary = employeeDto.Salary,
            AddressLine1 = employeeDto.AddressLine1,
            AddressLine2 = employeeDto.AddressLine2,
            City = employeeDto.City,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true
        };

        using (var connection = await CreateOpenConnectionAsync())
        {
            await connection.InsertAsync<Employee>(employee);
        }
    }

    public async Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate)
    {
        using (var connection = await CreateOpenConnectionAsync())
        {
            await connection.UpdateAsync<Employee>(new
            {
                Name = employeeUpdate.Name,
                UpdatedOn = DateTime.UtcNow
            },
            e => e.Id == employeeUpdate.EmployeeID);
        }
    }

    public async Task DeleteEmployeeById(EmployeeDeleteDto employeeDelete)
    {
        using (var connection = await CreateOpenConnectionAsync())
        {
            await connection.DeleteAsync<Employee>(e => e.Id == employeeDelete.EmployeeID);
        }
    }

    public async Task<List<GetEmployeesByCityDto>> GetEmployeesByCity(EmployeeCityQueryDto cityQuery)
    {
        var employees = await QueryAsync<Employee>(e => e.City == cityQuery.City);
        return employees.Select(e => new GetEmployeesByCityDto(e.Id, e.Name, e.City)).ToList();

    }

    public async Task<List<GetEmployeesByCityDto>> GetEmployeesByCity(EmployeeCityQueryDto cityQuery)
    {
        // Using RepoDB to create and open a SQL connection asynchronously
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Define the columns to select
            var fields = new[]
            {
            // Assuming the Employee class has properties that map directly to your database columns
            // Adjust the field names as necessary based on your actual database schema
            "Id", "Name", "City"
        };

            // Building the query dynamically
            var queryResult = await connection.QueryAsync<Employee>(
                where: e => e.City == cityQuery.City,
                fields: fields // Specify the fields to fetch
            );

            // Projecting the result to GetEmployeesByCityDto objects
            var employeesByCity = queryResult.Select(e => new GetEmployeesByCityDto(
                EmployeeID: e.Id,
                Name: e.Name,
                City: e.City
            )).ToList();

            return employeesByCity;
        }
    }


    public async Task<List<ProjectDto>> GetProjectsByEmployeeId(EmployeeProjectsQueryDto employeeProjectsQuery)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Define the columns to select
            var fields = Field.From(
                "Id", "Name", "StartDate", "EndDate", "Budget",
                "Status", "LogoSvg", "Notes", "Progress", "Priority", "EmployeeAssigned"
            );

            // Building the query dynamically with specific fields
            var projects = await connection.QueryAsync<Project>(
                where: e => e.EmployeeAssigned == employeeProjectsQuery.EmployeeID,
                fields: fields // Specify the fields to fetch
            );

            // Projecting the result to ProjectDto objects
            var projectDtos = projects.Select(p => new ProjectDto
            {
                ProjectID = p.Id,
                Name = p.Name,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Budget = p.Budget,
                Status = ConvertToByte(p.Status), // Convert enum to byte as needed
                LogoSvg = p.LogoSvg,
                Notes = p.Notes,
                Progress = p.Progress,
                Priority = ConvertToByte(p.Priority), // Convert enum to byte as needed
                EmployeeAssigned = p.EmployeeAssigned
            }).ToList();

            return projectDtos;
        }
    }

    private byte ConvertToByte(ProjectStatus status)
    {
        return (byte)status;
    }

    private byte ConvertToByte(ProjectPriority priority)
    {
        return (byte)priority;
    }


    public async Task<List<ProjectDto>> GetProjectsByCustomerId(CustomerProjectsQueryDto customerProjectsQuery)
    {
        // Initialize RepoDB SQL Server
        SqlServerBootstrap.Initialize();

        var projects = new List<ProjectDto>();

        // Define the SQL query to perform a join operation between Projects and ProjectCustomers
        string sql = @"
SELECT 
    p.Id AS ProjectID, 
    p.Name, 
    p.StartDate, 
    p.EndDate, 
    p.Budget, 
    p.Status, 
    p.LogoSvg, 
    p.Notes, 
    p.Progress, 
    p.Priority, 
    p.EmployeeAssigned 
FROM 
    Projects p
INNER JOIN 
    ProjectCustomers pc ON p.Id = pc.ProjectId
WHERE 
    pc.CustomerId = @CustomerId;";

        return ExecuteQueryAsync<ProjectDto>(sql, new { CustomerId = customerProjectsQuery.CustomerID });

    }

    public async Task<List<EmployeeDto>> FullTextSearch(FullTextSearchDto searchQuery)
    {
        using (var connection = await CreateOpenConnectionAsync())
        {
            // Define the query to leverage SQL Server's CONTAINS function for full-text search
            // Adjust the query according to your database schema, particularly the table and column names
            var query = @"
                SELECT 
                    Id AS EmployeeID, 
                    Name, 
                    Age, 
                    Department, 
                    HireDate, 
                    Salary, 
                    AddressLine1, 
                    AddressLine2, 
                    City
                FROM 
                    Employees
                WHERE 
                    CONTAINS((Name, Department), @SearchTerm)";

            // Execute the query asynchronously using RepoDB's ExecuteQueryAsync method
            var employees = await connection.ExecuteQueryAsync<EmployeeDto>(query, new { SearchTerm = searchQuery.SearchTerm });

            return employees.ToList();
        }
    }

    public async Task<List<EmployeesWithDynamicQueryDto>> GetEmployeesWithDynamicQuery(DynamicQueryDto query)
    {
        var sqlBuilder = new QueryBuilder();
        sqlBuilder.Select("Id AS EmployeeID, Name")
                  .From("Employees");

        // Dynamically add where conditions
        foreach (var filter in query.Filters)
        {
            var column = filter.Key;
            var value = filter.Value;
            if (value is int intValue)
            {
                sqlBuilder.Where($"{column} = @IntValue", new { IntValue = intValue });
            }
            else if (value is string stringValue)
            {
                sqlBuilder.Where($"{column} LIKE @StringValue", new { StringValue = $"%{stringValue}%" });
            }
            // Add more types as necessary
        }

        // Sort order
        var sortKey = query.SortOrder.Keys.FirstOrDefault();
        if (!string.IsNullOrEmpty(sortKey))
        {
            var sortOrder = query.SortOrder[sortKey] ? "ASC" : "DESC";
            sqlBuilder.OrderBy($"{sortKey} {sortOrder}");
        }
        else
        {
            // Default sort order if none provided
            sqlBuilder.OrderBy("Name ASC");
        }

        var sqlQuery = sqlBuilder.End().ToString();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Execute the query
            var dynamicQueryResults = await connection.ExecuteQueryAsync<EmployeesWithDynamicQueryDto>(sqlQuery, null);

            return dynamicQueryResults.ToList();
        }
    }


}

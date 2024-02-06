using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

public class EmployeeProjectRepositoryAdoNet : IEmployeeProjectRepository
{
    private readonly string _connectionString;

    public EmployeeProjectRepositoryAdoNet(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Executes a non-query command (e.g., INSERT, UPDATE, DELETE)
    protected async Task ExecuteNonQueryAsync(string commandText, CommandType commandType, SqlParameter[] parameters = null)
    {
        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(commandText, connection))
        {
            command.CommandType = commandType;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }

    // Executes a command that returns a single value
    protected async Task<object> ExecuteScalarAsync(string commandText, CommandType commandType, SqlParameter[] parameters = null)
    {
        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(commandText, connection))
        {
            command.CommandType = commandType;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            await connection.OpenAsync();
            return await command.ExecuteScalarAsync();
        }
    }

    // Executes a query and returns a list of a specific type
    protected async Task<List<T>> ExecuteQueryAsync<T>(string commandText, CommandType commandType, Func<SqlDataReader, T> convert, SqlParameter[] parameters = null)
    {
        var results = new List<T>();

        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(commandText, connection))
        {
            command.CommandType = commandType;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    results.Add(convert(reader));
                }
            }
        }

        return results;
    }

    public async Task AddEmployee(EmployeeAddDto employeeDto)
    {
        // SQL INSERT statement
        var commandText = @"
INSERT INTO Employees
(Name, Age, Department, HireDate, Salary, AddressLine1, AddressLine2, City)
VALUES
(@Name, @Age, @Department, @HireDate, @Salary, @AddressLine1, @AddressLine2, @City)";

        // Create parameters
        var parameters = new SqlParameter[]
        {
        new SqlParameter("@Name", SqlDbType.NVarChar) { Value = employeeDto.Name },
        new SqlParameter("@Age", SqlDbType.Int) { Value = (object)employeeDto.Age ?? DBNull.Value },
        new SqlParameter("@Department", SqlDbType.NVarChar) { Value = employeeDto.Department },
        new SqlParameter("@HireDate", SqlDbType.DateTime2) { Value = employeeDto.HireDate },
        new SqlParameter("@Salary", SqlDbType.Decimal) { Value = (object)employeeDto.Salary ?? DBNull.Value },
        new SqlParameter("@AddressLine1", SqlDbType.NVarChar) { Value = (object)employeeDto.AddressLine1 ?? DBNull.Value },
        new SqlParameter("@AddressLine2", SqlDbType.NVarChar) { Value = (object)employeeDto.AddressLine2 ?? DBNull.Value },
        new SqlParameter("@City", SqlDbType.NVarChar) { Value = employeeDto.City },
        };

        // Execute the INSERT command
        await ExecuteNonQueryAsync(commandText, CommandType.Text, parameters);
    }

    public async Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate)
    {
        // SQL UPDATE statement to update employee name and the UpdatedOn timestamp
        var commandText = @"
UPDATE Employees
SET Name = @Name, UpdatedOn = SYSDATETIME()
WHERE Id = @Id";

        // Create parameters for the SQL command
        var parameters = new SqlParameter[]
        {
        new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = employeeUpdate.EmployeeID },
        new SqlParameter("@Name", SqlDbType.NVarChar) { Value = employeeUpdate.Name }
        };

        // Execute the UPDATE command using the helper method
        await ExecuteNonQueryAsync(commandText, CommandType.Text, parameters);
    }



    public async Task DeleteEmployeeById(Guid employeeId)
    {
        var commandText = "DELETE FROM Employees WHERE Id = @Id";
        var parameters = new SqlParameter[]
        {
        new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = employeeId }
        };

        await ExecuteNonQueryAsync(commandText, CommandType.Text, parameters);
    }


    public async Task<List<GetEmployeesByCityDto>> GetEmployeesByCity(EmployeeCityQueryDto cityQuery)
    {
        // SQL SELECT statement to retrieve employees by city
        var commandText = @"
SELECT Id AS EmployeeID, Name, City
FROM Employees
WHERE City = @City";

        // Create parameters for the SQL command
        var parameters = new SqlParameter[]
        {
        new SqlParameter("@City", SqlDbType.NVarChar) { Value = cityQuery.City }
        };

        // Execute the query and convert the result set to a list of GetEmployeesByCityDto objects
        var employeesByCity = await ExecuteQueryAsync(commandText, CommandType.Text, reader => new GetEmployeesByCityDto
        {
            EmployeeID = reader.GetGuid(reader.GetOrdinal("EmployeeID")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            City = reader.GetString(reader.GetOrdinal("City"))
        }, parameters);

        return employeesByCity;
    }

    public async Task<List<ProjectDto>> GetProjectsByEmployeeId(EmployeeProjectsQueryDto employeeProjectsQuery)
    {
        // SQL SELECT statement to retrieve projects by employee ID
        var commandText = @"
SELECT Id AS ProjectID, Name, StartDate, EndDate, Budget, Status, LogoSvg, Notes, Progress, Priority, EmployeeAssigned
FROM Projects
WHERE EmployeeAssigned = @EmployeeID";

        // Create parameters for the SQL command
        var parameters = new SqlParameter[]
        {
        new SqlParameter("@EmployeeID", SqlDbType.UniqueIdentifier) { Value = employeeProjectsQuery.EmployeeID }
        };

        // Execute the query and convert the result set to a list of ProjectDto objects
        var projects = await ExecuteQueryAsync(commandText, CommandType.Text, reader => new ProjectDto
        {
            ProjectID = reader.GetGuid(reader.GetOrdinal("ProjectID")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
            Budget = reader.GetDecimal(reader.GetOrdinal("Budget")),
            Status = reader.GetByte(reader.GetOrdinal("Status")),
            LogoSvg = (byte[])reader.GetValue(reader.GetOrdinal("LogoSvg")),
            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
            Progress = reader.GetFloat(reader.GetOrdinal("Progress")),
            Priority = reader.GetByte(reader.GetOrdinal("Priority")),
            EmployeeAssigned = reader.IsDBNull(reader.GetOrdinal("EmployeeAssigned")) ? null : reader.GetGuid(reader.GetOrdinal("EmployeeAssigned"))
        }, parameters);

        return projects;
    }


    public async Task<List<ProjectDto>> GetProjectsByCustomerId(CustomerProjectsQueryDto customerProjectsQuery)
    {
        // SQL SELECT statement to retrieve projects by customer ID through the ProjectCustomers junction table
        var commandText = @"
SELECT p.Id AS ProjectID, p.Name, p.StartDate, p.EndDate, p.Budget, p.Status, p.LogoSvg, p.Notes, p.Progress, p.Priority, p.EmployeeAssigned
FROM Projects p
INNER JOIN ProjectCustomers pc ON p.Id = pc.ProjectId
WHERE pc.CustomerId = @CustomerID";

        // Create parameters for the SQL command
        var parameters = new SqlParameter[]
        {
        new SqlParameter("@CustomerID", SqlDbType.UniqueIdentifier) { Value = customerProjectsQuery.CustomerID }
        };

        // Execute the query and convert the result set to a list of ProjectDto objects
        var projects = await ExecuteQueryAsync(commandText, CommandType.Text, reader => new ProjectDto
        {
            ProjectID = reader.GetGuid(reader.GetOrdinal("ProjectID")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
            Budget = reader.GetDecimal(reader.GetOrdinal("Budget")),
            Status = Convert.ToByte(reader["Status"]), // Assuming Status is stored as an int in DB but represented as a byte in ProjectDto
            LogoSvg = reader["LogoSvg"] as byte[], // Handling potential DBNull values
            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
            Progress = reader.GetFloat(reader.GetOrdinal("Progress")),
            Priority = Convert.ToByte(reader["Priority"]), // Assuming Priority is stored as an int in DB but represented as a byte in ProjectDto
            EmployeeAssigned = reader.IsDBNull(reader.GetOrdinal("EmployeeAssigned")) ? null : reader.GetGuid(reader.GetOrdinal("EmployeeAssigned"))
        }, parameters);

        return projects;
    }



}

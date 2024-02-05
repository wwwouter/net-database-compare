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

    public async Task AddEmployee(EmployeeAddDto employee)
    {
        var sql = @"
INSERT INTO Employees (Id, Name, Age, Department, HireDate, Salary, AddressLine1, AddressLine2, City, CreatedOn, UpdatedOn) 
VALUES (@EmployeeID, @Name, @Age, @Department, @HireDate, @Salary, @AddressLine1, @AddressLine2, @City, @CreatedOn, @UpdatedOn)";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, new
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
    }



}

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient; // Or use the appropriate data provider's namespace

public class EmployeeProjectRepository : IEmployeeProjectRepository
{
    private readonly string _connectionString;

    public EmployeeProjectRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString); // Adjust for your DB provider
    }





}

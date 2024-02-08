using SqlKata;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper; // Assuming use of Dapper for execution
using System;

public class EmployeeProjectRepository : IEmployeeProjectRepository
{
    private readonly QueryFactory _db;
    private readonly IDbConnection _connection;

    public EmployeeProjectRepository(QueryFactory db, IDbConnection connection)
    {
        _db = db;
        _connection = connection;
    }

    public async Task AddEmployee(EmployeeAddDto employee)
    {
        // Example SqlKata query for adding an employee
        var query = _db.Query("Employees").AsInsert(new
        {
            // Map properties from EmployeeAddDto to database columns
            Id = employee.EmployeeID,
            Name = employee.Name,
            Age = employee.Age,
            Department = employee.Department,
            HireDate = employee.HireDate,
            Salary = employee.Salary,
            AddressLine1 = employee.AddressLine1,
            AddressLine2 = employee.AddressLine2,
            City = employee.City,
        });

        // Execute using Dapper or another execution strategy
        await _connection.ExecuteAsync(query.Sql, query.NamedBindings);
    }

    public async Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate)
    {
        // Example SqlKata query for updating an employee's name
        var query = _db.Query("Employees").Where("Id", employeeUpdate.EmployeeID).AsUpdate(new { Name = employeeUpdate.Name });

        // Execute using Dapper
        await _connection.ExecuteAsync(query.Sql, query.NamedBindings);
    }

    public async Task DeleteEmployeeById(EmployeeDeleteDto employeeDelete)
    {
        // Example SqlKata query for deleting an employee
        var query = _db.Query("Employees").Where("Id", employeeDelete.EmployeeID).AsDelete();

        // Execute using Dapper
        await _connection.ExecuteAsync(query.Sql, query.NamedBindings);
    }

    public async Task<List<GetEmployeesByCityDto>> GetEmployeesByCity(EmployeeCityQueryDto cityQuery)
    {
        // Example SqlKata query for fetching employees by city
        var query = _db.Query("Employees").Select("Id", "Name", "City").Where("City", cityQuery.City);

        // Execute and map to DTO using Dapper
        var employees = await _connection.QueryAsync<GetEmployeesByCityDto>(query.Sql, query.NamedBindings);
        return employees.AsList();
    }

    public async Task<List<EmployeesWithDynamicQueryDto>> GetEmployeesWithDynamicQuery(DynamicQueryDto query)
    {
        var dbQuery = _db.Query("Employees"); // Starting with a base query.

        // Dynamically applying filters.
        foreach (var filter in query.Filters)
        {
            // Apply filters based on the key and value.
            // Use LIKE for string comparisons to simulate EF's behavior for "contains" operation.
            if (filter.Value != null)
            {
                if (filter.Key.ToLower() == "age" && filter.Value is int ageValue)
                {
                    dbQuery = dbQuery.Where("Age", ageValue);
                }
                else
                {
                    // Assuming the filter.Value is string for simplicity.
                    // Adjust the code as necessary for other data types.
                    dbQuery = dbQuery.WhereLike(filter.Key, $"%{filter.Value}%");
                }
            }
        }

        // Apply sorting if specified.
        if (query.SortOrder != null && query.SortOrder.Count > 0)
        {
            foreach (var sort in query.SortOrder)
            {
                dbQuery = sort.Value ? dbQuery.OrderBy(sort.Key) : dbQuery.OrderByDesc(sort.Key);
            }
        }
        else
        {
            // Default sorting if no sort order is provided.
            dbQuery = dbQuery.OrderBy("Name");
        }

        var sqlResult = await dbQuery.GetAsync<dynamic>(); // Execute the query.

        // Projecting the dynamic result to EmployeesWithDynamicQueryDto.
        var result = sqlResult.Select(r => new EmployeesWithDynamicQueryDto
        {
            EmployeeID = r.Id,
            Name = r.Name,
            DynamicCriteria = query.Filters // Pass the filters as part of the DTO.
        }).ToList();

        return result;
    }


    public async Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomersWithFavoriteNumber(int favoriteNumber)
    {
        // SqlKata does not directly support JSON queries, so we'll use raw SQL for the JSON part
        // and utilize SqlKata for the rest of the query construction for demonstration purposes.
        var query = _db.Query("Customers")
                       .Select("Id as CustomerID",
                               "Name",
                               "Age",
                               "Email",
                               "PhoneNumber",
                               "AddressLine1",
                               "AddressLine2",
                               "City",
                               "Country",
                               "GeographicLocation.ToString() as GeographicLocation",
                               "LoyaltyPoints",
                               "LastPurchaseDate",
                               "Notes",
                               "JSONData")
                       .WhereRaw("JSON_VALUE(JSONData, '$.FavoriteNumbers') LIKE ?", $"%{favoriteNumber}%");

        // Using Dapper to execute the query
        var sqlResult = query.GetSql();
        var customers = await _connection.QueryAsync<CustomerBasedOnJsonPropertyDto>(sqlResult.Sql, sqlResult.NamedBindings);

        return customers.ToList();
    }



    public async Task<List<EmployeeHierarchyDto>> GetEmployeeHierarchy(EmployeeHierarchyQueryDto hierarchyQuery)
    {
        // Assuming EmployeeHierarchy has EmployeeId and ManagerId columns and we join with Employees table to get names
        var subquery = _db.Query("EmployeeHierarchy")
            .Join("Employees as Employee", "EmployeeHierarchy.EmployeeId", "Employee.Id")
            .Join("Employees as Manager", "EmployeeHierarchy.ManagerId", "Manager.Id")
            .Select(
                "EmployeeHierarchy.EmployeeId",
                "Employee.Name as EmployeeName",
                "EmployeeHierarchy.ManagerId",
                "Manager.Name as ManagerName"
            )
            .Where("EmployeeHierarchy.EmployeeId", hierarchyQuery.EmployeeID);

        var query = _db.Query().From(subquery.As("Hierarchy")).Select("*");

        // Note: You may need to execute this as raw SQL if the query is complex or recursive
        var result = await _connection.QueryAsync<EmployeeHierarchyDto>(query.ToSql(), query.GetBindings());

        return result.AsList();
    }

}

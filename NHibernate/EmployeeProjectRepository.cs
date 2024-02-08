using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EmployeeProjectRepository : IEmployeeProjectRepository
{
    private readonly ISession _session;

    public EmployeeProjectRepository(ISession session)
    {
        _session = session;
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
            IsActive = true // IsActive should be true upon creation
        };

        await _session.SaveAsync(employee);
        await transaction.CommitAsync();

    }

    public async Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate)
    {
        using (var transaction = _session.BeginTransaction())
        {
            var employee = await _session.Query<Employee>()
                                         .Where(e => e.Id == employeeUpdate.EmployeeID)
                                         .FirstOrDefaultAsync();

            if (employee != null)
            {
                employee.Name = employeeUpdate.Name;
                // Set UpdatedOn to the current UTC date and time
                employee.UpdatedOn = DateTime.UtcNow;

                await _session.SaveOrUpdateAsync(employee);
                await transaction.CommitAsync();
            }
            //          // HQL query to update employee name directly
            // var hql = "update Employee set Name = :newName where Id = :employeeId";
            // await _session.CreateQuery(hql)
            //               .SetParameter("newName", employeeUpdate.Name)
            //               .SetParameter("employeeId", employeeUpdate.EmployeeID)
            //               .ExecuteUpdateAsync();
        }
    }

    public async Task DeleteEmployeeById(EmployeeDeleteDto employeeDelete)
    {
        using (var transaction = _session.BeginTransaction())
        {
            var employee = await _session.Query<Employee>()
                                         .Where(e => e.Id == employeeDelete.EmployeeID)
                                         .FirstOrDefaultAsync();

            if (employee != null)
            {
                _session.Delete(employee);
                await transaction.CommitAsync();
            }
        }
    }

    public async Task<List<GetEmployeesByCityDto>> GetEmployeesByCity(EmployeeCityQueryDto cityQuery)
    {
        // Use LINQ to query the Employees table and select employees by city
        var employeesByCity = await _session.Query<Employee>()
            .Where(e => e.City == cityQuery.City)
            .Select(e => new GetEmployeesByCityDto
            {
                EmployeeID = e.Id,
                Name = e.Name,
                City = e.City
            })
            .ToListAsync();

        return employeesByCity;
    }

    public async Task<List<ProjectDto>> GetProjectsByEmployeeId(EmployeeProjectsQueryDto employeeProjectsQuery)
    {
        // Query the Projects table to find projects assigned to the specified employee ID
        var projectsByEmployeeId = await _session.Query<Project>()
            .Where(p => p.EmployeeAssigned == employeeProjectsQuery.EmployeeID)
            .Select(p => new ProjectDto
            {
                ProjectID = p.Id,
                Name = p.Name,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Budget = p.Budget,
                Status = (byte)p.Status, // Assuming Status is an enum and you want to convert it to its byte value
                LogoSvg = p.LogoSvg,
                Notes = p.Notes,
                Progress = p.Progress,
                Priority = (byte)p.Priority, // Similarly, assuming Priority is an enum
                EmployeeAssigned = p.EmployeeAssigned
            })
            .ToListAsync();

        return projectsByEmployeeId;
    }

    public async Task<List<ProjectDto>> GetProjectsByCustomerId(CustomerProjectsQueryDto customerProjectsQuery)
    {
        // Querying the Project and ProjectCustomer tables to find projects associated with the given customer ID
        var projectsByCustomerId = await _session.Query<ProjectCustomer>()
            .Where(pc => pc.CustomerId == customerProjectsQuery.CustomerID)
            .Select(pc => pc.Project)
            .Select(p => new ProjectDto
            {
                ProjectID = p.Id,
                Name = p.Name,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Budget = p.Budget,
                Status = (byte)p.Status, // Assuming Status is an enum and converting it to byte
                LogoSvg = p.LogoSvg,
                Notes = p.Notes,
                Progress = p.Progress,
                Priority = (byte)p.Priority, // Similarly, assuming Priority is an enum
                EmployeeAssigned = p.EmployeeAssigned
            })
            .ToListAsync();

        return projectsByCustomerId;
    }

    public async Task<List<EmployeeDto>> FullTextSearch(FullTextSearchDto searchQuery)
    {
        var searchTerm = $"%{searchQuery.SearchTerm}%";

        // Assuming Employee entity is correctly mapped and the session is configured
        var employees = await _session.Query<Employee>()
            .Where(e => e.Name.Contains(searchQuery.SearchTerm) || e.Department.Contains(searchQuery.SearchTerm))
            .Select(e => new EmployeeDto
            {
                EmployeeID = e.Id,
                Name = e.Name,
                Age = e.Age,
                Department = e.Department,
                HireDate = e.HireDate,
                Salary = e.Salary,
                AddressLine1 = e.AddressLine1,
                AddressLine2 = e.AddressLine2,
                City = e.City
            }).ToListAsync();

        return employees;
    }

    public async Task<List<EmployeeProjectOuterJoinDto>> GetEmployeeProjectsWithOuterJoin()
    {
        // Perform a left outer join between Employees and Projects
        var result = await _session.Query<Employee>()
            // Use LINQ's DefaultIfEmpty to simulate a left outer join
            .SelectMany(
                employee => _session.Query<Project>().Where(project => project.EmployeeAssigned == employee.Id).DefaultIfEmpty(),
                (employee, project) => new EmployeeProjectOuterJoinDto
                {
                    EmployeeID = employee.Id,
                    ProjectID = project != null ? project.Id : (Guid?)null // If there's no project, ProjectID is null
                }
            ).ToListAsync();

        return result;
    }

    public async Task<List<EmployeeSubqueryDto>> GetEmployeesWithSubquery()
    {
        // Define the budget threshold
        decimal budgetThreshold = 100000M;

        // Perform the query
        var employeesWithHighBudgetProjects = await _session.Query<Employee>()
            .Where(e => e.AssignedProjects.Any(p => p.Budget > budgetThreshold))
            .Select(e => new EmployeeSubqueryDto
            {
                EmployeeID = e.Id,
                Name = e.Name
            })
            .ToListAsync();

        return employeesWithHighBudgetProjects;
    }

    public async Task EditJsonData(EditJsonDataDto editJsonDataDto)
    {
        // Start a transaction
        using (var transaction = _session.BeginTransaction())
        {
            // Fetch the customer entity by ID
            var customer = await _session.Query<Customer>()
                                          .FirstOrDefaultAsync(c => c.Id == editJsonDataDto.EntityId);

            if (customer != null)
            {
                // Assuming JSONData is a string property containing JSON data
                // Update the JSON data with the provided value
                customer.JSONData = JsonSerializer.Serialize(editJsonDataDto.UpdatedJsonData);

                // Update the customer entity in the session
                await _session.SaveOrUpdateAsync(customer);

                // Commit the transaction
                await transaction.CommitAsync();
            }
        }
    }

    public async Task AppendNumberToJsonData(AppendNumberToJsonDataDto appendNumberDto)
    {
        // SQL query to update JSON data
        var sql = @"
UPDATE Customers
SET JSONData = JSON_MODIFY(JSONData, '$.FavoriteNumbers', JSON_QUERY(
    CASE 
        WHEN JSON_VALUE(JSONData, '$.FavoriteNumbers') IS NOT NULL 
        THEN CONCAT(SUBSTRING(JSON_QUERY(JSONData, '$.FavoriteNumbers'), 1, LEN(JSON_QUERY(JSONData, '$.FavoriteNumbers')) - 1), ',', @NumberToAppend, ']')
        ELSE JSON_QUERY('[' + CAST(@NumberToAppend AS VARCHAR(20)) + ']')
    END
))
WHERE Id = @EntityId";

        // Execute the raw SQL update
        using (var transaction = _session.BeginTransaction())
        {
            await _session.CreateSQLQuery(sql)
                          .SetParameter("NumberToAppend", appendNumberDto.NumberToAppend)
                          .SetParameter("EntityId", appendNumberDto.EntityId)
                          .ExecuteUpdateAsync();

            await transaction.CommitAsync();
        }
    }

    public async Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomerBasedOnJsonProperty(JsonPropertyQueryDto jsonPropertyQuery)
    {
        // Construct the raw SQL query using SQL Server's JSON functions
        string sqlQuery = $@"
        SELECT
            Id AS CustomerID,
            Name,
            Age,
            Email,
            PhoneNumber,
            AddressLine1,
            AddressLine2,
            City,
            Country,
            GeographicLocation.ToString() AS GeographicLocation,
            LoyaltyPoints,
            LastPurchaseDate,
            Notes,
            JSONData
        FROM
            Customers
        WHERE
            JSON_VALUE(JSONData, '$.{jsonPropertyQuery.JsonPropertyName}') = :Value";

        // Execute the raw SQL query using NHibernate's session
        var queryResult = await _session.CreateSQLQuery(sqlQuery)
            .SetParameter("Value", jsonPropertyQuery.Value)
            .SetResultTransformer(Transformers.AliasToBean<CustomerBasedOnJsonPropertyDto>())
            .ListAsync<CustomerBasedOnJsonPropertyDto>();

        return queryResult.ToList();
    }

    public async Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomersWithFavoriteNumber(int favoriteNumber)
    {
        // Raw SQL query leveraging SQL Server's JSON functions to find the favorite number in an array
        string sqlQuery = $@"
        SELECT
            Id AS CustomerID,
            Name,
            Age,
            Email,
            PhoneNumber,
            AddressLine1,
            AddressLine2,
            City,
            Country,
            GeographicLocation.ToString() AS GeographicLocation,
            LoyaltyPoints,
            LastPurchaseDate,
            Notes,
            JSONData
        FROM
            Customers
        WHERE
            JSON_VALUE(JSONData, '$.FavoriteNumbers') LIKE :FavoriteNumber
            OR JSON_QUERY(JSONData, '$.FavoriteNumbers') LIKE '%':FavoriteNumber'%'";

        // Execute the raw SQL query using NHibernate's session
        var queryResult = await _session.CreateSQLQuery(sqlQuery)
            .SetParameter("FavoriteNumber", favoriteNumber)
            .SetResultTransformer(Transformers.AliasToBean<CustomerBasedOnJsonPropertyDto>())
            .ListAsync<CustomerBasedOnJsonPropertyDto>();

        return queryResult.ToList();
    }

    public async Task<List<EmployeeHierarchyDto>> GetEmployeeHierarchy(EmployeeHierarchyQueryDto hierarchyQuery)
    {
        // Define the CTE SQL query
        string cteQuery = @"
    ;WITH EmployeeCTE AS (
        SELECT e.Id, e.Name AS EmployeeName, eh.ManagerId, m.Name AS ManagerName
        FROM Employees e
        LEFT JOIN EmployeeHierarchy eh ON e.Id = eh.EmployeeId
        LEFT JOIN Employees m ON eh.ManagerId = m.Id
        WHERE e.Id = :employeeId
        UNION ALL
        SELECT e.Id, e.Name, eh.ManagerId, m.Name
        FROM Employees e
        INNER JOIN EmployeeHierarchy eh ON e.Id = eh.EmployeeId
        INNER JOIN EmployeeCTE ecte ON eh.ManagerId = ecte.Id
        LEFT JOIN Employees m ON eh.ManagerId = m.Id
    )
    SELECT Id AS EmployeeId, EmployeeName, ManagerId, ManagerName FROM EmployeeCTE";

        // Execute the CTE query using NHibernate
        var employeeHierarchy = await _session.CreateSQLQuery(cteQuery)
            .SetParameter("employeeId", hierarchyQuery.EmployeeID)
            .SetResultTransformer(Transformers.AliasToBean<EmployeeHierarchyDto>())
            .ListAsync<EmployeeHierarchyDto>();

        return employeeHierarchy.ToList();
    }

    public async Task AddEmployeeWithPartialData(EmployeePartialAddDto employeePartial)
    {
        // Create a new Employee instance with partial data from the DTO
        var employee = new Employee
        {
            Name = employeePartial.Name,
            Age = employeePartial.Age,
            Department = "DefaultDepartment", // Assuming a default value since it's partial data
            HireDate = DateTime.UtcNow, // Assuming current date/time for HireDate
            IsActive = true // Assuming IsActive should be true by default
        };

        // Use NHibernate session to save the new employee
        using (var transaction = _session.BeginTransaction())
        {
            await _session.SaveAsync(employee);
            await transaction.CommitAsync();
        }
    }

    public async Task<ProjectWithEmployee> GetProjectWithAssignedEmployee(Guid projectId)
    {
        // Perform a LINQ query to retrieve the project with its assigned employee
        var query = from project in _session.Query<Project>()
                    where project.Id == projectId
                    select new ProjectWithEmployee
                    {
                        Id = project.Id.ToString(),
                        Name = project.Name,
                        EmployeeAssigned = project.Employee != null ? new EmployeeInfo
                        {
                            Id = project.Employee.Id.ToString(),
                            Name = project.Employee.Name,
                            Department = project.Employee.Department
                        } : null
                    };

        // Execute the query and return the result
        var result = await query.FirstOrDefaultAsync();

        return result;
    }

    public async Task RunTwoUpdatesInSingleTransaction(SingleOperationTransactionDto data)
    {
        // Start a transaction using NHibernate's session
        using (var transaction = _session.BeginTransaction())
        {
            try
            {
                // Fetch and update the first employee
                var employee1 = await _session.GetAsync<Employee>(data.Id1);
                if (employee1 != null)
                {
                    employee1.Name = data.Name1;
                    await _session.SaveOrUpdateAsync(employee1);
                }
                else
                {
                    // Optionally handle the case where the employee doesn't exist
                }

                // Fetch and update the second employee
                var employee2 = await _session.GetAsync<Employee>(data.Id2);
                if (employee2 != null)
                {
                    employee2.Name = data.Name2;
                    await _session.SaveOrUpdateAsync(employee2);
                }
                else
                {
                    // Optionally handle the case where the employee doesn't exist
                }

                // Commit the transaction if both updates succeed
                await transaction.CommitAsync();
            }
            catch
            {
                // Rollback the transaction in case of any error
                await transaction.RollbackAsync();
                throw; // Rethrow the exception to handle it outside this method
            }
        }
    }



}

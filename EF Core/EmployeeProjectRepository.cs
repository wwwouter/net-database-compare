using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

public class EmployeeProjectRepository : IEmployeeProjectRepository
{
    private readonly AppDbContext _context;

    public EmployeeProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        var dbContextTransaction = await _context.Database.BeginTransactionAsync();
        return new EfCoreTransaction(dbContextTransaction);
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
            CreatedOn = employeeDto.CreatedOn,
            UpdatedOn = employeeDto.UpdatedOn,
            IsActive = true // Needs to be set explicitly if it's not nullable and has a default value
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate)
    {
        var employee = await _context.Employees.FindAsync(employeeUpdate.EmployeeID);
        if (employee != null)
        {
            employee.Name = employeeUpdate.Name;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteEmployeeById(EmployeeDeleteDto employeeDelete)
    {
        var employee = await _context.Employees.FindAsync(employeeDelete.EmployeeID);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<GetEmployeesByCityDto>> GetEmployeesByCity(EmployeeCityQueryDto cityQuery)
    {
        return await _context.Employees
                             .Where(e => e.City == cityQuery.City)
                             .Select(e => new GetEmployeesByCityDto(e.Id, e.Name, e.City))
                             .ToListAsync();
    }

    public async Task<List<ProjectDto>> GetProjectsByEmployeeId(EmployeeProjectsQueryDto employeeProjectsQuery)
    {
        return await _context.Projects
                             .Where(p => p.EmployeeAssigned == employeeProjectsQuery.EmployeeID)
                             .Select(p => new ProjectDto
                             {
                                 ProjectID = p.Id,
                                 Name = p.Name,
                                 StartDate = p.StartDate,
                                 EndDate = p.EndDate,
                                 Budget = p.Budget,
                                 Status = (byte)p.Status,
                                 LogoSvg = p.LogoSvg,
                                 Notes = p.Notes,
                                 Progress = p.Progress,
                                 Priority = (byte)p.Priority,
                                 EmployeeAssigned = p.EmployeeAssigned
                             })
                             .ToListAsync();
    }

    public async Task<List<ProjectDto>> GetProjectsByCustomerId(CustomerProjectsQueryDto customerProjectsQuery)
    {
        // Querying the database to find all projects associated with the given customer ID.
        var projects = await _context.Projects
            .Where(p => _context.ProjectCustomers
                .Any(pc => pc.CustomerId == customerProjectsQuery.CustomerID && pc.ProjectId == p.Id))
            .Select(p => new ProjectDto
            {
                ProjectID = p.Id,
                Name = p.Name,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Budget = p.Budget,
                Status = (byte)p.Status, // Assuming you will convert the enum to its byte value here
                LogoSvg = p.LogoSvg,
                Notes = p.Notes,
                Progress = p.Progress,
                Priority = (byte)p.Priority, // Similarly, converting the enum to its byte value
                EmployeeAssigned = p.EmployeeAssigned,
                CreatedOn = p.CreatedOn,
                UpdatedOn = p.UpdatedOn
            })
            .ToListAsync();

        return projects;
    }

    public async Task<List<EmployeeDto>> FullTextSearch(FullTextSearchDto searchQuery)
    {
        var employees = await _context.Employees
            .Where(e => EF.Functions.FreeText(e.Name, searchQuery.SearchTerm) ||
                        EF.Functions.FreeText(e.Department, searchQuery.SearchTerm))
            .Select(e => new EmployeeDto
            {
                EmployeeID = e.Id,
                Name = e.Name,
                Age = e.Age,
                Gender = e.Gender,
                Department = e.Department,
                HireDate = e.HireDate,
                Salary = e.Salary ?? 0M, // Assuming Salary is a nullable decimal and providing a default value if null
                AddressLine1 = e.AddressLine1,
                AddressLine2 = e.AddressLine2,
                City = e.City
            })
            .ToListAsync();

        return employees;
    }

    public async Task<List<EmployeeProjectOuterJoinDto>> GetEmployeeProjectsWithOuterJoin()
    {
        // Query to perform a left outer join between Employees and Projects
        var query = from employee in _context.Employees
                    join project in _context.Projects
                    on employee.Id equals project.EmployeeAssigned into projectGroup
                    from subProject in projectGroup.DefaultIfEmpty() // Ensures it's a left outer join
                    select new EmployeeProjectOuterJoinDto
                    {
                        EmployeeID = employee.Id,
                        ProjectID = subProject != null ? subProject.Id : (Guid?)null
                    };

        return await query.ToListAsync();
    }


    public async Task<List<EmployeeSubqueryDto>> GetEmployeesWithSubquery()
    {
        // Define a threshold for project budgets
        var budgetThreshold = 100000M;

        var employeesWithHighBudgetProjects = await _context.Employees
            .Where(e => _context.Projects
                .Any(p => p.EmployeeAssigned == e.Id && p.Budget > budgetThreshold))
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
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == editJsonDataDto.EntityId);

        if (customer != null)
        {
            // Assuming JsonDataDto can be directly serialized into JSON string
            // You might need to adjust serialization settings based on your requirements
            customer.JSONData = JsonSerializer.Serialize(editJsonDataDto.UpdatedJsonData);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomersWithFavoriteNumber(int favoriteNumber)
    {
        return await _context.Customers
            .Where(c => EF.Functions.JsonContains(c.JSONData, JsonSerializer.Serialize(favoriteNumber), "$.FavoriteNumbers"))
            .Select(c => new CustomerBasedOnJsonPropertyDto
            {
                CustomerID = c.Id,
                Name = c.Name,
                Age = c.Age,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                AddressLine1 = c.AddressLine1,
                AddressLine2 = c.AddressLine2,
                City = c.City,
                Country = c.Country,
                GeographicLocation = c.GeographicLocation,
                LoyaltyPoints = c.LoyaltyPoints,
                LastPurchaseDate = c.LastPurchaseDate,
                Notes = c.Notes,
                JSONData = c.JSONData
            })
            .ToListAsync();
    }


    public async Task AppendNumberToJsonData(AppendNumberToJsonDataDto appendNumberDto)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == appendNumberDto.EntityId);

        if (customer != null)
        {
            var jsonData = JsonSerializer.Deserialize<JsonDataDto>(customer.JSONData) ?? new JsonDataDto();
            jsonData.FavoriteNumbers.Add(appendNumberDto.NumberToAppend);
            customer.JSONData = JsonSerializer.Serialize(jsonData);
            await _context.SaveChangesAsync();
        }
    }


    public async Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomerBasedOnJsonProperty(JsonPropertyQueryDto jsonPropertyQuery)
    {
        var query = _context.Customers
            .Where(c => EF.Functions.JsonValue(c.JSONData, $"$.{jsonPropertyQuery.JsonPropertyName}") == jsonPropertyQuery.Value)
            .Select(c => new CustomerBasedOnJsonPropertyDto
            {
                CustomerID = c.Id,
                Name = c.Name,
                Age = c.Age,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                AddressLine1 = c.AddressLine1,
                AddressLine2 = c.AddressLine2,
                City = c.City,
                Country = c.Country,
                GeographicLocation = c.GeographicLocation,
                LoyaltyPoints = c.LoyaltyPoints,
                LastPurchaseDate = c.LastPurchaseDate,
                Notes = c.Notes,
                JSONData = c.JSONData
            });

        return await query.ToListAsync();
    }

    public async Task<List<EmployeeHierarchyDto>> GetEmployeeHierarchy(EmployeeHierarchyQueryDto hierarchyQuery)
    {
        var cteQuery = @"
WITH RECURSIVE EmployeeCTE AS (
    SELECT e.Id AS EmployeeId, e.Name AS EmployeeName, eh.ManagerId, m.Name AS ManagerName
    FROM Employees e
    LEFT JOIN EmployeeHierarchy eh ON e.Id = eh.EmployeeId
    LEFT JOIN Employees m ON eh.ManagerId = m.Id
    WHERE e.Id = {0}
    UNION ALL
    SELECT e.Id, e.Name, eh.ManagerId, m.Name
    FROM Employees e
    INNER JOIN EmployeeHierarchy eh ON e.Id = eh.EmployeeId
    INNER JOIN EmployeeCTE ecte ON eh.ManagerId = ecte.EmployeeId
    LEFT JOIN Employees m ON eh.ManagerId = m.Id
)
SELECT * FROM EmployeeCTE";

        var employeeHierarchies = await _context.EmployeeHierarchies
            .FromSqlRaw(cteQuery, hierarchyQuery.EmployeeID)
            .Select(eh => new EmployeeHierarchyDto
            {
                EmployeeId = eh.EmployeeId,
                ManagerId = eh.ManagerId,
                EmployeeName = eh.EmployeeName,
                ManagerName = eh.ManagerName
            }).ToListAsync();

        return employeeHierarchies;
    }


    public async Task AddEmployeeWithPartialData(EmployeePartialAddDto employeePartial)
    {
        var employee = new Employee
        {
            // Id is auto-generated by the database with DEFAULT newid(), no need to set it here.
            // modelBuilder.Entity<Employee>()
            // .Property(e => e.Id)
            // .HasDefaultValueSql("newid()");
            Name = employeePartial.Name,
            Age = employeePartial.Age,
            Department = "DefaultDepartment",
            HireDate = DateTime.UtcNow,

            CreatedOn = DateTime.UtcNow, // Explicitly set to ensure it matches the intended default behavior
                                         // or
                                         //    modelBuilder.Entity<Employee>()
                                         //       .Property(e => e.CreatedOn)
                                         //       .HasDefaultValueSql("SYSDATETIME()")
                                         //       .ValueGeneratedOnAdd();
            IsActive = true // Explicitly set to ensure it matches the intended default behavior
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
    }

    public async Task RunTwoUpdatesInSingleTransaction(SingleOperationTransactionDto data)
    {
        var transaction = _context.Database.BeginTransaction();
        try
        {
            var employee1 = await _context.Employees.FindAsync(data.Id1);
            if (employee1 != null)
            {
                employee1.Name = data.Name1;
            }

            var employee2 = await _context.Employees.FindAsync(data.Id2);
            if (employee2 != null)
            {
                employee2.Name = data.Name2;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }

    public async Task BulkInsertEmployees(IEnumerable<EmployeeBulkInsertDto> employeeDtos)
    {
        // Use EF Core Bulk Extensions for more performance

        var employees = employeeDtos.Select(dto => new Employee
        {
            Id = dto.EmployeeID,
            Name = dto.Name,
            Age = dto.Age,
            Department = dto.Department,
            HireDate = dto.HireDate,
            Salary = dto.Salary,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            City = dto.City,
            CreatedOn = DateTime.UtcNow,
            IsActive = true
        });

        _context.ChangeTracker.AutoDetectChangesEnabled = false; // Improve performance for bulk insert

        await _context.Employees.AddRangeAsync(employees);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.AutoDetectChangesEnabled = true; // Re-enable auto-detect changes after the bulk insert
    }

    public async Task BulkUpdateEmployees(IEnumerable<EmployeeBulkUpdateDto> employeeDtos)
    {
        // Use EF Core Bulk Extensions for more performance
        //      var employeesToUpdate = employeeDtos.Select(dto => new Employee
        // {
        //     Id = dto.EmployeeID,
        //     Name = dto.Name,
        // }).ToList();

        // await _context.BulkUpdateAsync(employeesToUpdate);

        _context.ChangeTracker.AutoDetectChangesEnabled = false; // Improve performance

        foreach (var dto in employeeDtos)
        {
            var employee = await _context.Employees.FindAsync(dto.EmployeeID);
            if (employee != null)
            {
                employee.Name = dto.Name;
                // Apply other updates as necessary
            }
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    public async Task<List<EmployeesWithDynamicQueryDto>> GetEmployeesWithDynamicQuery(DynamicQueryDto query)
    {
        // Starting with a base query
        IQueryable<Employee> baseQuery = _context.Employees;

        // Dynamically applying filters
        foreach (var filter in query.Filters)
        {
            switch (filter.Key.ToLower())
            {
                case "name":
                    baseQuery = baseQuery.Where(e => EF.Functions.Like(e.Name, $"%{filter.Value}%"));
                    break;
                case "department":
                    baseQuery = baseQuery.Where(e => EF.Functions.Like(e.Department, $"%{filter.Value}%"));
                    break;
                case "age":
                    if (filter.Value is int age)
                    {
                        baseQuery = baseQuery.Where(e => e.Age == age);
                    }
                    break;
                default:
                    break;
            }
        }

        // Preparing for sorting - defaulting to Name if no sort criteria provided
        var sortProperty = query.SortOrder.FirstOrDefault();
        if (string.IsNullOrEmpty(sortProperty.Key))
        {
            baseQuery = baseQuery.OrderBy(e => e.Name);
        }
        else
        {
            switch (sortProperty.Key.ToLower())
            {
                case "name":
                    baseQuery = sortProperty.Value ? baseQuery.OrderBy(e => e.Name) : baseQuery.OrderByDescending(e => e.Name);
                    break;
                case "age":
                    baseQuery = sortProperty.Value ? baseQuery.OrderBy(e => e.Age) : baseQuery.OrderByDescending(e => e.Age);
                    break;
                default:
                    baseQuery = baseQuery.OrderBy(e => e.Name); // Fallback sorting
                    break;
            }
        }

        // Projecting the final query to DTO
        var result = await baseQuery.Select(e => new EmployeesWithDynamicQueryDto
        {
            EmployeeID = e.Id,
            Name = e.Name,
            DynamicCriteria = query.Filters
        }).ToListAsync();

        return result;
    }


    public async Task<PagedResultDto<EmployeeDto>> GetEmployeesPagedAndSorted(PagingAndSortingQueryDto query)
    {
        // Base query from Employees table
        IQueryable<Employee> baseQuery = _context.Employees;

        // Applying sorting
        switch (query.SortBy.ToLower())
        {
            case "name":
                baseQuery = query.Ascending ? baseQuery.OrderBy(e => e.Name) : baseQuery.OrderByDescending(e => e.Name);
                break;
            case "department":
                baseQuery = query.Ascending ? baseQuery.OrderBy(e => e.Department) : baseQuery.OrderByDescending(e => e.Department);
                break;
            case "age":
                baseQuery = query.Ascending ? baseQuery.OrderBy(e => e.Age) : baseQuery.OrderByDescending(e => e.Age);
                break;
            // Default sorting by Name if no valid sort by provided
            default:
                baseQuery = baseQuery.OrderBy(e => e.Name);
                break;
        }

        // Count total items for pagination metadata before applying pagination
        int totalCount = await baseQuery.CountAsync();

        // Applying pagination
        var pagedQuery = baseQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize);

        // Projecting to DTO
        var items = await pagedQuery.Select(e => new EmployeeDto
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

        // Constructing the result with pagination metadata
        var result = new PagedResultDto<EmployeeDto>(items, totalCount);

        return result;
    }

    public async Task<List<EmployeeSelfJoinDto>> GetEmployeeManagers()
    {
        // Performing a self-join on the Employees table to link each employee with their manager
        var query = from employee in _context.Employees
                    join manager in _context.Employees on employee.EmployeeHierarchy.ManagerId equals manager.Id into managerGroup
                    from m in managerGroup.DefaultIfEmpty() // This ensures that employees without managers are included in the results
                    select new EmployeeSelfJoinDto
                    {
                        EmployeeID = employee.Id,
                        ManagerID = m != null ? m.Id : (Guid?)null, // If there's no manager, ManagerID will be null
                        EmployeeName = employee.Name,
                        ManagerName = m != null ? m.Name : null // If there's no manager, ManagerName will be null
                    };

        return await query.ToListAsync();
    }

    public async Task<decimal> GetTotalBudgetForProjects()
    {
        // Calculate the total budget by summing the Budget column of all projects
        var totalBudget = await _context.Projects.SumAsync(p => p.Budget);

        return totalBudget;
    }

    public async Task<List<ProjectSummaryDto>> GetProjectSummaries()
    {
        return await _context.ProjectSummaries.ToListAsync();
    }

    public async Task<List<EmployeeDto>> CallStoredProcedure(StoredProcedureQueryDto query)
    {
        // Assuming the stored procedure name is "GetEmployeesByDepartment"
        var procedureName = "GetEmployeesByDepartment";
        var departmentParam = new Microsoft.Data.SqlClient.SqlParameter("@Department", query.Department);

        // Execute the stored procedure and map the results to the EmployeeDto
        var employees = await _context.Employees
            .FromSqlRaw($"EXEC {procedureName} @Department", departmentParam)
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



    public async Task<List<CustomerSpatialQueryDto>> GetCustomersNearLocation(SpatialQueryDto query)
    {
        // Ensure the geometry factory's SRID matches your database SRID for geographic locations.
        // 4326 is commonly used for GPS coordinates.
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        // Create a point representing the query location
        var queryLocation = geometryFactory.CreatePoint(new Coordinate(query.Longitude, query.Latitude));

        // Query customers within the specified distance from the query location
        var customers = await _context.Customers
            .Where(c => c.GeographicLocation.IsWithinDistance(queryLocation, query.Distance))
            .Select(c => new CustomerSpatialQueryDto
            {
                CustomerID = c.Id,
                Name = c.Name,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                AddressLine1 = c.AddressLine1,
                AddressLine2 = c.AddressLine2,
                City = c.City,
                Country = c.Country,
                GeographicLocation = c.GeographicLocation,
                LoyaltyPoints = c.LoyaltyPoints,
                LastPurchaseDate = c.LastPurchaseDate,
                Notes = c.Notes
            })
            .ToListAsync();

        return customers;
    }



}

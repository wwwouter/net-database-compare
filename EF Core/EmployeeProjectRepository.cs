using Microsoft.EntityFrameworkCore;
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
            UpdatedOn = null
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


}

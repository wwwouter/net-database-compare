using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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





}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

public interface IEmployeeProjectRepository
{
    // Adds a new employee to the database.
    Task AddEmployee(EmployeeAddDto employee);

    // Updates the name of an employee based on the provided employee ID.
    Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate);

    // Deletes an employee from the database based on the provided employee ID.
    Task DeleteEmployeeById(EmployeeDeleteDto employeeDelete);

    // Retrieves a list of employees based on the provided city.
    Task<List<GetEmployeesByCityDto>> GetEmployeesByCity(EmployeeCityQueryDto cityQuery);

    // Retrieves a list of projects assigned to a specific employee.
    Task<List<ProjectDto>> GetProjectsByEmployeeId(EmployeeProjectsQueryDto employeeProjectsQuery);

    // Retrieves a list of projects associated with a specific customer.
    Task<List<ProjectDto>> GetProjectsByCustomerId(CustomerProjectsQueryDto customerProjectsQuery);

    // Performs a full-text search across relevant tables/columns.
    Task<List<EmployeeDto>> FullTextSearch(FullTextSearchDto searchQuery);

    // Demonstrates the use of an outer join in a query.
    Task<List<EmployeeProjectOuterJoinDto>> GetEmployeeProjectsWithOuterJoin();

    // Showcases a select within a select query.
    Task<List<EmployeeSubqueryDto>> GetEmployeesWithSubquery();

    // Edits the entire JSONData column for a specific entity.
    Task EditJsonData(EditJsonDataDto editJsonDataDto);

    // Appends a number to the favoriteNumbers array within the JSONData column of a specific entity.
    Task AppendNumberToJsonData(AppendNumberToJsonDataDto appendNumberDto);

    // Example method for selecting entities based on a condition within JSONData
    Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomersWithFavoriteNumber(int favoriteNumber);

    // Selects based on a property within a JSON column.
    Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomerBasedOnJsonProperty(JsonPropertyQueryDto jsonPropertyQuery);

    // Demonstrates the use of Common Table Expressions (CTE).
    Task<List<EmployeeHierarchyDto>> GetEmployeeHierarchy(EmployeeHierarchyQueryDto hierarchyQuery);

    // Demonstrates handling of partial object creation, specifically with the IsActive flag.
    Task AddEmployeeWithPartialData(EmployeePartialAddDto employeePartial);

    // Executes a single operation within a transaction.
    Task PerformSingleOperationInTransaction(SingleOperationTransactionDto operation);

    // Executes multiple operations within a single transaction.
    Task PerformMultipleOperationsInTransaction(MultipleOperationsTransactionDto operations);

    // Inserts a bulk list of entities efficiently.
    Task BulkInsertEmployees(IEnumerable<EmployeeBulkInsertDto> employees);

    // Updates a bulk list of entities efficiently.
    Task BulkUpdateEmployees(IEnumerable<EmployeeBulkUpdateDto> employees);

    // Dynamically generates a query based on a set of filters and sort criteria.
    Task<List<EmployeesWithDynamicQueryDto>> GetEmployeesWithDynamicQuery(DynamicQueryDto query);

    // Retrieves a paginated list of employees with sorting.
    Task<PagedResultDto<EmployeeDto>> GetEmployeesPagedAndSorted(PagingAndSortingQueryDto query);

    // Demonstrates the use of a self-join.
    Task<List<EmployeeSelfJoinDto>> GetEmployeeManagers();

    // Uses an aggregate function in a query.
    Task<decimal> GetTotalBudgetForProjects();

    // Selects data from a view.
    Task<List<ProjectSummaryDto>> GetProjectSummaries();

    // Calls a stored procedure and handles its results.
    Task<List<EmployeeDto>> CallStoredProcedure(StoredProcedureQueryDto query);

    // Handles database migrations with a file-based approach.
    Task ApplyMigrations();

    // Performs a spatial data selection, e.g., finding customers within a certain distance.
    Task<List<CustomerSpatialQueryDto>> GetCustomersNearLocation(SpatialQueryDto query);

    // Appends to an array within a JSON object.
    Task AppendToArrayInJsonObject(JsonArrayAppendDto arrayAppend);
}


public record JsonDataDto
{
    public string Name { get; init; }
    public string? Category { get; init; }
    public List<int> FavoriteNumbers { get; init; } = new();
}

public record EditJsonDataDto(Guid EntityId, JsonDataDto UpdatedJsonData);

public record AppendNumberToJsonDataDto(Guid EntityId, int NumberToAppend);

public record EmployeeAddDto(
    Guid EmployeeID,
    string Name,
    int Age,
    string Department,
    DateTime HireDate,
    decimal Salary,
    string? AddressLine1,
    string? AddressLine2,
    string City,
    DateTime CreatedOn,
    DateTime? UpdatedOn);

public record EmployeeUpdateNameDto(Guid EmployeeID, string Name);

public record EmployeeDeleteDto(Guid EmployeeID);

public record EmployeeCityQueryDto(string City);

public record EmployeeProjectsQueryDto(Guid EmployeeID);

public record CustomerProjectsQueryDto(Guid CustomerID);

public record FullTextSearchDto(string SearchTerm);

public record EmployeeProjectOuterJoinDto(Guid EmployeeID, Guid? ProjectID);

public record EmployeeSubqueryDto(Guid EmployeeID, string Name);

public record JsonEditDto(Guid EntityID, string JsonPropertyName, string NewValue);

public record JsonPropertyQueryDto(string JsonPropertyName, string Value);

public record EmployeeHierarchyQueryDto(Guid EmployeeID);

public record EmployeePartialAddDto(string Name, int Age);

public record SingleOperationTransactionDto(Action Operation);

public record MultipleOperationsTransactionDto(IEnumerable<Action> Operations);

public record EmployeeBulkInsertDto(IEnumerable<EmployeeAddDto> Employees);

public record EmployeeBulkUpdateDto(IEnumerable<EmployeeUpdateNameDto> Employees);

public record DynamicQueryDto(Dictionary<string, object?> Filters, Dictionary<string, bool> SortOrder);

public record PagingAndSortingQueryDto(int PageNumber, int PageSize, string SortBy, bool Ascending);

public record EmployeeSelfJoinDto(Guid EmployeeID, Guid? ManagerID);

public record ProjectSummaryDto(Guid ProjectID, string Name, decimal TotalBudget);

public record StoredProcedureQueryDto(string ProcedureName, Dictionary<string, object?> Parameters);

public record SpatialQueryDto(double Latitude, double Longitude, double Distance);

public record JsonArrayAppendDto(Guid EntityID, string ArrayPropertyName, string NewValue);

public record EmployeeDto(
    Guid EmployeeID,
    string Name,
    int? Age,
    string Department,
    DateTime HireDate,
    decimal? Salary,
    string? AddressLine1,
    string? AddressLine2,
    string City);

public record ProjectDto(
    Guid ProjectID,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    decimal Budget,
    byte Status,
    byte[] LogoSvg,
    string? Notes,
    float Progress,
    byte Priority,
    Guid? EmployeeAssigned);

public record CustomerBasedOnJsonPropertyDto(
    Guid CustomerID,
    string Name,
    int? Age,
    string Email,
    string PhoneNumber,
    string? AddressLine1,
    string? AddressLine2,
    string City,
    string Country,
    Point? GeographicLocation,
    int LoyaltyPoints,
    DateTime? LastPurchaseDate,
    string? Notes,
    string JSONData);

public record GetEmployeesByCityDto(Guid EmployeeID, string Name, string City);

public record EmployeesWithDynamicQueryDto(Guid EmployeeID, string Name, Dictionary<string, object?> DynamicCriteria);

public record PagedResultDto<T>(IEnumerable<T> Items, int TotalCount) where T : class;


public record EmployeeHierarchyDto
{
    public Guid EmployeeId { get; init; }
    public Guid? ManagerId { get; init; }
    public string EmployeeName { get; init; }
    public string? ManagerName { get; init; }
}


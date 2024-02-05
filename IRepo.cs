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

    // Executes two different updates in a single transaction.
    Task RunTwoUpdatesInSingleTransaction(SingleOperationTransactionDto data);

    // Method to start a new transaction
    Task<ITransaction> BeginTransactionAsync();

    // Executes a single operation within a transaction.
    Task Operation1InATransaction(Guid id, string name);

    // Executes a single operation within a transaction.
    Task Operation2InATransaction(Guid id, string name);

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

    // Performs a spatial data selection, e.g., finding customers within a certain distance.
    Task<List<CustomerSpatialQueryDto>> GetCustomersNearLocation(SpatialQueryDto query);
}

public interface ITransaction : IDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}


public interface ITransactionService
{
    Task<ITransaction> BeginTransactionAsync();

}



public record JsonDataDto
{
    public string Name { get; init; }
    public string? Category { get; init; }
    public List<int> FavoriteNumbers { get; init; } = new();
}

public record EditJsonDataDto(
    [Required] Guid EntityId,
    [Required] JsonDataDto UpdatedJsonData
);

public record AppendNumberToJsonDataDto(
    [Required] Guid EntityId,
    [Required, Range(0, int.MaxValue)] int NumberToAppend
); public record EmployeeAddDto(
    [Required] Guid EmployeeID,
    [Required, StringLength(50)] string Name,
    [Required, Range(1, 120)] int Age,
    [Required, StringLength(20)] string Department,
    [Required] DateTime HireDate,
    [Required, Range(0.01, double.MaxValue)] decimal Salary,
    StringLength(50)] string? AddressLine1,
    [StringLength(50)] string? AddressLine2,
    [Required, StringLength(30)] string City,
    [Required] DateTime CreatedOn,
    DateTime? UpdatedOn
);

public record EmployeeUpdateNameDto(
    [Required] Guid EmployeeID,
    [Required, StringLength(50)] string Name
);

public record EmployeeDeleteDto([Required] Guid EmployeeID);

public record EmployeeCityQueryDto([Required, StringLength(30)] string City);

public record EmployeeProjectsQueryDto([Required] Guid EmployeeID);

public record CustomerProjectsQueryDto([Required] Guid CustomerID);

public record FullTextSearchDto([Required] string SearchTerm);

public record EmployeeProjectOuterJoinDto(Guid EmployeeID, Guid? ProjectID);

public record EmployeeSubqueryDto(Guid EmployeeID, string Name);

public record JsonEditDto(
    [Required] Guid EntityID,
    [Required, StringLength(50)] string JsonPropertyName,
    [Required] string NewValue
);

public record JsonPropertyQueryDto(
    [Required, StringLength(50)] string JsonPropertyName,
    [Required] string Value
);

public record EmployeeHierarchyQueryDto([Required] Guid EmployeeID);

public record EmployeePartialAddDto(
    [Required, StringLength(50)] string Name,
    [Required, Range(1, 120)] int Age
);

public record EmployeeBulkInsertDto(IEnumerable<EmployeeAddDto> Employees);

public record EmployeeBulkUpdateDto(IEnumerable<EmployeeUpdateNameDto> Employees);

public record DynamicQueryDto(
    [Required] Dictionary<string, object?> Filters,
    [Required] Dictionary<string, bool> SortOrder
);

public record PagingAndSortingQueryDto(
    [Required, Range(1, int.MaxValue)] int PageNumber,
    [Required, Range(1, 100)] int PageSize,
    [Required] string SortBy,
    [Required] bool Ascending
);

public record EmployeeSelfJoinDto(
    [Required] Guid EmployeeID,
    Guid? ManagerID
);

public record ProjectSummaryDto(
    [Required] Guid ProjectID,
    [Required, StringLength(50)] string Name,
    [Required, Range(0.01, double.MaxValue)] decimal TotalBudget,
    [Required] byte Status,
    [Required] DateTime StartDate,
    [Required] DateTime EndDate,
    [Required] float Progress,
    [Required] byte Priority,
    [StringLength(50)] string EmployeeAssignedName,
    [Required, Range(0, int.MaxValue)] int NumberOfCustomers
);


public record StoredProcedureQueryDto([Required, StringLength(20)] string Department);

public record SpatialQueryDto(
    [Required, Range(-90, 90)] double Latitude,
    [Required, Range(-180, 180)] double Longitude,
    [Required, Range(0, double.MaxValue)] double Distance
);


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

public record SingleOperationTransactionDto(
    [Required] Guid id1,
    [Required, StringLength(50)] string name1,
    [Required] Guid id2,
    [Required, StringLength(50)] string name2
);

public record CustomerSpatialQueryDto(
     Guid CustomerID,
     string Name,
     string Email,
     string PhoneNumber,
     string? AddressLine1,
     string? AddressLine2,
     string City,
     string Country,
    Point? GeographicLocation,
     int LoyaltyPoints,
    DateTime? LastPurchaseDate,
    string? Notes
);
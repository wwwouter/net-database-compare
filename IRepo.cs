using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    // Edits a property within a JSON column.
    Task EditPropertyInJson(JsonEditDto jsonEdit);

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
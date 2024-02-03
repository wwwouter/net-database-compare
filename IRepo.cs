using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEmployeeProjectRepository
{
    // Adds a new employee to the database.
    Task AddEmployeeAsync(Employee employee);

    // Updates the name of an employee based on the provided employee ID.
    Task UpdateEmployeeNameAsync(Guid employeeId, string newName);

    // Deletes an employee from the database based on the provided employee ID.
    Task DeleteEmployeeByIdAsync(Guid employeeId);

    // Retrieves a list of employees based on the provided city.
    Task<List<Employee>> GetEmployeesByCityAsync(string city);

    // Retrieves a list of projects assigned to a specific employee.
    Task<List<Project>> GetProjectsByEmployeeIdAsync(Guid employeeId);

    // Retrieves a list of projects associated with a specific customer.
    Task<List<Project>> GetProjectsByCustomerIdAsync(Guid customerId);

    // Performs a full-text search across relevant tables/columns.
    Task<List<Employee>> FullTextSearchAsync(string searchTerm);

    // Demonstrates the use of an outer join in a query.
    Task<List<EmployeeProject>> GetEmployeeProjectsWithOuterJoinAsync();

    // Showcases a select within a select query.
    Task<List<Employee>> GetEmployeesWithSubqueryAsync();

    // Edits a property within a JSON column.
    Task EditPropertyInJsonAsync(Guid entityId, string jsonPropertyName, string newValue);

    // Selects based on a property within a JSON column.
    Task<List<Customer>> SelectBasedOnJsonPropertyAsync(string jsonPropertyName, string value);

    // Demonstrates the use of Common Table Expressions (CTE).
    Task<List<Employee>> GetEmployeeHierarchyAsync(Guid employeeId);

    // Demonstrates handling of partial object creation, specifically with the IsActive flag.
    Task AddEmployeeWithPartialDataAsync(Employee employee);

    // Executes a single operation within a transaction.
    Task PerformSingleOperationInTransactionAsync(Action operation);

    // Executes multiple operations within a single transaction.
    Task PerformMultipleOperationsInTransactionAsync(IEnumerable<Action> operations);

    // Inserts a bulk list of entities efficiently.
    Task BulkInsertEmployeesAsync(IEnumerable<Employee> employees);

    // Updates a bulk list of entities efficiently.
    Task BulkUpdateEmployeesAsync(IEnumerable<Employee> employees);

    // Dynamically generates a query based on a set of filters and sort criteria.
    Task<List<Employee>> DynamicQueryAsync(Dictionary<string, object> filters, Dictionary<string, bool> sortOrder);

    // Retrieves a paginated list of employees with sorting.
    Task<(List<Employee>, int)> GetEmployeesPagedAndSortedAsync(int pageNumber, int pageSize, string sortBy, bool ascending);

    // Demonstrates the use of a self-join.
    Task<List<Employee>> GetEmployeeManagersAsync();

    // Uses an aggregate function in a query.
    Task<decimal> GetTotalBudgetForProjectsAsync();

    // Selects data from a view.
    Task<List<ProjectSummary>> GetProjectSummariesAsync();

    // Calls a stored procedure and handles its results.
    Task<List<Employee>> CallStoredProcedureAsync(string procedureName, Dictionary<string, object> parameters);

    // Handles database migrations with a file-based approach.
    Task ApplyMigrationsAsync();

    // Performs a spatial data selection, e.g., finding customers within a certain distance.
    Task<List<Customer>> GetCustomersNearLocationAsync(double latitude, double longitude, double distance);

    // Appends to an array within a JSON object.
    Task AppendToArrayInJsonObjectAsync(Guid entityId, string arrayPropertyName, string newValue);
}

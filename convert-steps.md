

I'm writing a demo app to compare different data access packages. I already implemented EF Core and now I want to create similar code with ADO.NET. MS SQL Server is the database. 


<!-- - If relevant to have something like EF Core Entities, create them (use Entities.cs a basis), otherwise explain why not. -->
- Create EfCoreTransaction equivalent and TransactionService, based on ITransaction.
- Create empty EmployeeProjectRepository
- Fill EmployeeProjectRepository one method at a time.
    - Implement Task AddEmployee(EmployeeAddDto employee); using helper methods.
    - Implement Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate);  using helper methods. Make sure UpdatedOn is set correctly.
    - Implement Task DeleteEmployeeById(EmployeeDeleteDto employeeDelete);
    - Create some helper methods in EmployeeProjectRepository to make executing queries easier.
    - Implement Task<List<GetEmployeesByCityDto>> GetEmployeesByCity(EmployeeCityQueryDto cityQuery);  using helper methods if possible.
    - Implement Task<List<ProjectDto>> GetProjectsByEmployeeId(EmployeeProjectsQueryDto employeeProjectsQuery);  using helper methods if possible.
    - Implement Task<List<ProjectDto>> GetProjectsByCustomerId(CustomerProjectsQueryDto customerProjectsQuery);  using helper methods if possible.
    - Implement Task<List<EmployeeDto>> FullTextSearch(FullTextSearchDto searchQuery);  using helper methods if possible.
    - Implement Task<List<EmployeeProjectOuterJoinDto>> GetEmployeeProjectsWithOuterJoin();  using helper methods if possible.
    - Implement Task<List<EmployeeSubqueryDto>> GetEmployeesWithSubquery();  using helper methods if possible.
    - Implement Task EditJsonData(EditJsonDataDto editJsonDataDto);  using helper methods if possible.
    - Implement Task AppendNumberToJsonData(AppendNumberToJsonDataDto appendNumberDto);  using helper methods if possible.
    - Implement Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomerBasedOnJsonProperty(JsonPropertyQueryDto jsonPropertyQuery);  using helper methods if possible.
    - Implement Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomersWithFavoriteNumber(int favoriteNumber);  using helper methods if possible.
    - Implement Task<List<EmployeeHierarchyDto>> GetEmployeeHierarchy(EmployeeHierarchyQueryDto hierarchyQuery);  using helper methods if possible.
    - Can you update or add helper methods to make executing queries easier?
    - Implement Task AddEmployeeWithPartialData(EmployeePartialAddDto employeePartial);  using helper methods if possible.
    - Implement Task<ProjectWithEmployee> GetProjectWithAssignedEmployee(Guid projectId);  using helper methods if possible.
    - Implement Task RunTwoUpdatesInSingleTransaction(SingleOperationTransactionDto data);  using helper methods if possible.
    - Implement Task Operation1InATransaction(Guid id, string name);  using helper methods if possible., Task Operation2InATransaction(Guid id, string name);  using helper methods if possible. and EmployeeService equivalent
    - Implement Task BulkInsertEmployees(IEnumerable<EmployeeBulkInsertDto> employees);  using helper methods if possible.
    - Implement Task BulkUpdateEmployees(IEnumerable<EmployeeBulkUpdateDto> employees);  using helper methods if possible.
    - Implement Task<List<EmployeesWithDynamicQueryDto>> GetEmployeesWithDynamicQuery(DynamicQueryDto query);  using helper methods if possible.
    - Implement Task<PagedResultDto<EmployeeDto>> GetEmployeesPagedAndSorted(PagingAndSortingQueryDto query);  using helper methods if possible.
    - Implement Task<List<EmployeeSelfJoinDto>> GetEmployeeManagers();  using helper methods if possible.
    - Implement Task<decimal> GetTotalBudgetForProjects();  using helper methods if possible.
    - Implement Task<List<ProjectSummaryDto>> GetProjectSummaries();  using helper methods if possible.
    - Implement Task<List<EmployeeDto>> CallStoredProcedure(StoredProcedureQueryDto query);  using helper methods if possible.
    - Implement Task<List<CustomerSpatialQueryDto>> GetCustomersNearLocation(SpatialQueryDto query);  using helper methods if possible.
- Create AppDbContext equivalent if necessary/relevant.
- Create a Program.cs for a Web Application Using ASP.NET Core, based on EF Core Program.cs. Focus only on builder.Services and running the migrations.
- Check: How to make sure UpdatedOn is set correctly? Preferably without using triggers and manual actions. 

## Skip
- Create MigrationService using Dapper, based on EF Core MigrationService and AppliedMigrations and MigrationLock tables. 
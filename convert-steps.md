

I'm writing a demo app to compare different data access packages. I already implemented EF Core and now I want to create similar code with ADO.NET. MS SQL Server is the database. 


<!-- - If relevant to have something like EF Core Entities, create them (use Entities.cs a basis), otherwise explain why not. -->
<!-- - Create EfCoreTransaction equivalent and TransactionService, based on ITransaction. -->
<!-- - Create empty EmployeeProjectRepository -->
- Fill EmployeeProjectRepository one method at a time.
    <!-- - Implement Task AddEmployee(EmployeeAddDto employee); -->
        <!-- - Do I need to set `IsActive = true `? Is there another way? Remember `IsActive bit NOT NULL DEFAULT 1` -->
        <!-- - How about CreatedOn and UpdatedOn? -->
    <!-- - Implement Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate);  -->
        <!-- - Do I need to fetch the entire entity first? Would prefer just one query. -->
        <!-- - Make sure UpdatedOn is set correctly here and also in all other updates. -->
    <!-- - Implement Task DeleteEmployeeById(EmployeeDeleteDto employeeDelete); -->
    <!-- - Create some helper methods in EmployeeProjectRepository to make executing queries easier. -->
    <!-- - Implement Task<List<GetEmployeesByCityDto>> GetEmployeesByCity(EmployeeCityQueryDto cityQuery);  using helper methods if possible. -->
        <!-- - Is it SELECTing all columns? If so, can you make it SELECT only the necessary columns? -->
    <!-- - Implement Task<List<ProjectDto>> GetProjectsByEmployeeId(EmployeeProjectsQueryDto employeeProjectsQuery);  using helper methods if possible. -->
    <!-- - Implement Task<List<ProjectDto>> GetProjectsByCustomerId(CustomerProjectsQueryDto customerProjectsQuery);  using helper methods if possible. -->
    <!-- - Implement Task<List<EmployeeDto>> FullTextSearch(FullTextSearchDto searchQuery);  using helper methods if possible. -->
        <!-- - Is there SQL injection possible?  -->
    <!-- - Implement Task<List<EmployeeProjectOuterJoinDto>> GetEmployeeProjectsWithOuterJoin();  using helper methods if possible. -->
    <!-- - Implement Task<List<EmployeeSubqueryDto>> GetEmployeesWithSubquery();  using helper methods if possible. -->
    <!-- - Implement Task EditJsonData(EditJsonDataDto editJsonDataDto);  using helper methods if possible. -->
    <!-- - Implement Task AppendNumberToJsonData(AppendNumberToJsonDataDto appendNumberDto);  using helper methods if possible. -->
        <!-- - Is there a way to do this without fetching the entire entity and do it in one query? -->
    <!-- - Implement Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomerBasedOnJsonProperty(JsonPropertyQueryDto jsonPropertyQuery);  using helper methods if possible. -->
    <!-- - Implement Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomersWithFavoriteNumber(int favoriteNumber);  using helper methods if possible. -->
    <!-- - Implement Task<List<EmployeeHierarchyDto>> GetEmployeeHierarchy(EmployeeHierarchyQueryDto hierarchyQuery);  using helper methods if possible. -->
    <!-- - Can you update or add helper methods to make executing queries easier? -->
    <!-- - Implement Task AddEmployeeWithPartialData(EmployeePartialAddDto employeePartial);  using helper methods if possible. -->
    <!-- - Implement Task<ProjectWithEmployee> GetProjectWithAssignedEmployee(Guid projectId);  using helper methods if possible. -->
    <!-- - Implement Task RunTwoUpdatesInSingleTransaction(SingleOperationTransactionDto data);  using helper methods if possible. -->
    <!-- - Implement Task Operation1InATransaction(Guid id, string name);  and  Task Operation2InATransaction(Guid id, string name);  the repository methods Operation1InATransaction and Operation2InATransaction to not manage transactions themselves but to perform the required operations within the transaction context managed by EmployeeService.  -->
    <!-- - Implement Task BulkInsertEmployees(IEnumerable<EmployeeBulkInsertDto> employees);  using helper methods if possible. -->
    <!-- - Implement Task BulkUpdateEmployees(IEnumerable<EmployeeBulkUpdateDto> employees);  using helper methods if possible. -->
    <!-- - Implement Task<List<EmployeesWithDynamicQueryDto>> GetEmployeesWithDynamicQuery(DynamicQueryDto query);  using helper methods if possible. -->
    <!-- - Implement Task<PagedResultDto<EmployeeDto>> GetEmployeesPagedAndSorted(PagingAndSortingQueryDto query);  using helper methods if possible. -->
    <!-- - Implement Task<List<EmployeeSelfJoinDto>> GetEmployeeManagers();  using helper methods if possible. -->
    <!-- - Implement Task<decimal> GetTotalBudgetForProjects();  using helper methods if possible. -->
    - Implement Task<List<ProjectSummaryDto>> GetProjectSummaries();  using helper methods if possible.
    - Implement Task<List<EmployeeDto>> CallStoredProcedure(StoredProcedureQueryDto query);  using helper methods if possible.
    - Implement Task<List<CustomerSpatialQueryDto>> GetCustomersNearLocation(SpatialQueryDto query);  using helper methods if possible.
- Create AppDbContext equivalent if necessary/relevant.
- Create a Program.cs for a Web Application Using ASP.NET Core, based on EF Core Program.cs. Focus only on builder.Services and running the migrations.
- Check: How to make sure UpdatedOn is set correctly? Preferably without using triggers and manual actions. 

## Skip
- Create MigrationService using Dapper, based on EF Core MigrationService and AppliedMigrations and MigrationLock tables. 
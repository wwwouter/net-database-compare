

I'm writing a demo app to compare different data access packages. I already implemented EF Core and now I want to create similar code with Dapper. MS SQL Server is the database. 


<!-- - Create Entities. -->
<!-- - Create EfCoreTransaction equivalent. -->
<!-- - Create TransactionService. -->
<!-- - Create empty EmployeeProjectRepository -->
- Fill EmployeeProjectRepository one method at a time.
    <!-- - Implement Task AddEmployee(EmployeeAddDto employee); -->
    <!-- - Implement Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate); -->
    <!-- - Implement Task DeleteEmployeeById(EmployeeDeleteDto employeeDelete); -->
    - Implement Task<List<GetEmployeesByCityDto>> GetEmployeesByCity(EmployeeCityQueryDto cityQuery);
    - Implement Task<List<ProjectDto>> GetProjectsByEmployeeId(EmployeeProjectsQueryDto employeeProjectsQuery);
    - Implement Task<List<ProjectDto>> GetProjectsByCustomerId(CustomerProjectsQueryDto customerProjectsQuery);
    - Implement Task<List<EmployeeDto>> FullTextSearch(FullTextSearchDto searchQuery);
    - Implement Task<List<EmployeeProjectOuterJoinDto>> GetEmployeeProjectsWithOuterJoin();
    - Implement Task<List<EmployeeSubqueryDto>> GetEmployeesWithSubquery();
    - Implement Task EditJsonData(EditJsonDataDto editJsonDataDto);
    - Implement Task AppendNumberToJsonData(AppendNumberToJsonDataDto appendNumberDto);
    - Implement Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomersWithFavoriteNumber(int favoriteNumber);
    - Implement Task<List<CustomerBasedOnJsonPropertyDto>> SelectCustomerBasedOnJsonProperty(JsonPropertyQueryDto jsonPropertyQuery);
    - Implement Task<List<EmployeeHierarchyDto>> GetEmployeeHierarchy(EmployeeHierarchyQueryDto hierarchyQuery);
    - Implement Task AddEmployeeWithPartialData(EmployeePartialAddDto employeePartial);
    - Implement Task RunTwoUpdatesInSingleTransaction(SingleOperationTransactionDto data);
    - Implement Task<ITransaction> BeginTransactionAsync();
    - Implement Task Operation1InATransaction(Guid id, string name);
    - Implement Task Operation2InATransaction(Guid id, string name);
    - Implement Task BulkInsertEmployees(IEnumerable<EmployeeBulkInsertDto> employees);
    - Implement Task BulkUpdateEmployees(IEnumerable<EmployeeBulkUpdateDto> employees);
    - Implement Task<List<EmployeesWithDynamicQueryDto>> GetEmployeesWithDynamicQuery(DynamicQueryDto query);
    - Implement Task<PagedResultDto<EmployeeDto>> GetEmployeesPagedAndSorted(PagingAndSortingQueryDto query);
    - Implement Task<List<EmployeeSelfJoinDto>> GetEmployeeManagers();
    - Implement Task<decimal> GetTotalBudgetForProjects();
    - Implement Task<List<ProjectSummaryDto>> GetProjectSummaries();
    - Implement Task<List<EmployeeDto>> CallStoredProcedure(StoredProcedureQueryDto query);
    - Implement Task<List<CustomerSpatialQueryDto>> GetCustomersNearLocation(SpatialQueryDto query);
- Create MigrationService.
- Create AppDbContext equivalent.
- Create Program.cs.
- How to make sure CreatedOn and UpdatedOn are set correctly? Preferably without using triggers and manual actions.
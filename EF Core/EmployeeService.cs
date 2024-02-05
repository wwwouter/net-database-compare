public class EmployeeService
{
    private readonly IEmployeeProjectRepository _repository;
    private readonly ITransactionService _transactionService;

    public EmployeeService(ITransactionService transactionService, IEmployeeProjectRepository repository)
    {
        _transactionService = transactionService;
        _repository = repository;
    }

    public async Task UpdateTwoEmployeesInTransaction(Guid employeeId1, string newName1, Guid employeeId2, string newName2)
    {
        using var transaction = await _transactionService.BeginTransactionAsync();

        try
        {
            await _repository.Operation1InATransaction(employeeId1, newName1);
            await _repository.Operation2InATransaction(employeeId2, newName2);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw; // Rethrow the exception for further handling
        }
    }
}

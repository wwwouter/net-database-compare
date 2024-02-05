public class EmployeeService
{
    private readonly IEmployeeProjectRepository _repository;

    public EmployeeService(IEmployeeProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task UpdateTwoEmployeesInTransaction(Guid employeeId1, string newName1, Guid employeeId2, string newName2)
    {
        using var transaction = await _repository.BeginTransactionAsync();

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
    {
        using var transaction = await _repository.BeginTransactionAsync();

        try
        {

            Guid employeeId1 = // Obtain or define employee ID 1
           string newName1 = "New Name 1";
            Guid employeeId2 = // Obtain or define employee ID 2
            string newName2 = "New Name 2";

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

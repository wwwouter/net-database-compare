public class EfCoreTransaction : ITransaction
{
    private readonly IDbContextTransaction _dbContextTransaction;

    public EfCoreTransaction(IDbContextTransaction dbContextTransaction)
    {
        _dbContextTransaction = dbContextTransaction;
    }

    public async Task CommitAsync()
    {
        await _dbContextTransaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _dbContextTransaction.RollbackAsync();
    }

    public void Dispose()
    {
        _dbContextTransaction.Dispose();
    }
}

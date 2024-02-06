public class NhTransaction : ITransaction
{
    private readonly ITransaction _transaction;

    public NhTransaction(ITransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task CommitAsync()
    {
        if (!_transaction.IsActive)
            throw new InvalidOperationException("Transaction is not active.");

        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        if (!_transaction.IsActive)
            throw new InvalidOperationException("Transaction is not active.");

        await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        if (_transaction.IsActive)
        {
            _transaction.Rollback();
        }
        _transaction.Dispose();
    }
}

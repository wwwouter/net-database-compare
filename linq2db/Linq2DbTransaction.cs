using LinqToDB.Data;
using System;
using System.Threading.Tasks;

public class Linq2DbTransaction : ITransaction
{
    private readonly DataConnectionTransaction _transaction;

    public Linq2DbTransaction(DataConnection dataConnection)
    {
        _transaction = dataConnection.BeginTransaction();
    }

    public async Task CommitAsync()
    {
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }
}

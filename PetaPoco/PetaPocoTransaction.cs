using PetaPoco;
using System;
using System.Threading.Tasks;

public class PetaPocoTransaction : ITransaction
{
    private readonly ITransaction _transaction;
    private readonly Database _database;

    public PetaPocoTransaction(Database database)
    {
        _database = database;
        _transaction = _database.GetTransaction(); // Automatically starts a transaction
    }

    public async Task CommitAsync()
    {
        // PetaPoco's transaction commit is synchronous, but we can wrap it in a Task for async compatibility
        await Task.Run(() => _transaction.Complete());
    }

    public async Task RollbackAsync()
    {
        // Since PetaPoco manages rollback through IDisposable, there's nothing to do here.
        // The transaction is rolled back if Complete() is not called, but we provide a method for clarity.
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        // Dispose the transaction to commit or rollback changes
        _transaction.Dispose();
    }
}

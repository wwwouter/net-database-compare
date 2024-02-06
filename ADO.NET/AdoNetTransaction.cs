using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

public class AdoNetTransaction : ITransaction
{
    private readonly SqlTransaction _transaction;
    private readonly SqlConnection _connection;

    public AdoNetTransaction(SqlConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _transaction = _connection.BeginTransaction();
    }

    public async Task CommitAsync()
    {
        await Task.Run(() => _transaction.Commit());
    }

    public async Task RollbackAsync()
    {
        await Task.Run(() => _transaction.Rollback());
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
    }
}

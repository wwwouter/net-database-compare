using System;
using System.Data;
using System.Threading.Tasks;

public class DapperTransaction : ITransaction
{
    private readonly IDbTransaction _transaction;
    private readonly IDbConnection _connection;
    private bool _disposed;

    public DapperTransaction(IDbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _connection.Open(); // Ensure the connection is open
        _transaction = _connection.BeginTransaction();
    }

    public async Task CommitAsync()
    {
        try
        {
            _transaction.Commit();
            await Task.CompletedTask; // Placeholder to match async signature
        }
        catch
        {
            // Optionally, log or handle errors here
            throw;
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            _transaction.Rollback();
            await Task.CompletedTask; // Placeholder to match async signature
        }
        catch
        {
            // Optionally, log or handle errors here
            throw;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Close();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

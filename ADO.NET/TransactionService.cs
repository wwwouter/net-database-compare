using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

public class TransactionService : ITransactionService
{
    private readonly string _connectionString;

    public TransactionService(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return new AdoNetTransaction(connection);
    }
}

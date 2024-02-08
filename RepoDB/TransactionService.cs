using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

public class TransactionService : ITransactionService
{
    private readonly string _connectionString;

    public TransactionService(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        // Open a new SqlConnection
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Begin a transaction from the open connection
        var dbTransaction = await connection.BeginTransactionAsync();

        // Return a new RepoDbTransaction wrapping the IDbTransaction
        return new RepoDbTransaction(dbTransaction);
    }
}

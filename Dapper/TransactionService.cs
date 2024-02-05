using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient; // Or the appropriate namespace for your DB provider

public class TransactionService : ITransactionService
{
    private readonly string _connectionString;

    public TransactionService(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        // Create and open a new database connection
        IDbConnection connection = new SqlConnection(_connectionString);

        // Ensuring the connection is open before starting a transaction is important,
        // but since we're returning a DapperTransaction that opens the connection in its constructor,
        // we don't need to open it here. DapperTransaction will handle it.
        // This is a placeholder to match async signature. In a real-world scenario, you might directly open the connection asynchronously if your DB driver supports it.
        await Task.CompletedTask;

        // Initialize a new DapperTransaction which will manage the IDbTransaction lifecycle
        return new DapperTransaction(connection);
    }
}

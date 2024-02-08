using PetaPoco;
using System.Threading.Tasks;

public class PetaPocoTransactionService : ITransactionService
{
    private readonly Database _database;

    public PetaPocoTransactionService(Database database)
    {
        _database = database;
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        // The creation of a PetaPocoTransaction starts the transaction
        return await Task.FromResult(new PetaPocoTransaction(_database));
    }
}

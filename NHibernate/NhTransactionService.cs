public class NhTransactionService : ITransactionService
{
    private readonly ISession _session;

    public NhTransactionService(ISession session)
    {
        _session = session;
    }

    public async Task<ITransaction> BeginTransactionAsync()
    {
        return new NhTransaction(await _session.BeginTransactionAsync());
    }
}

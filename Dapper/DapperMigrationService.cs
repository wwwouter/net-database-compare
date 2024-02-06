using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

// Basic version, EF Core version is more complete
public class DapperMigrationService
{
    private readonly string _connectionString;
    private readonly string _migrationScriptsPath;

    public DapperMigrationService(string connectionString, string migrationScriptsPath)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _migrationScriptsPath = migrationScriptsPath ?? throw new ArgumentNullException(nameof(migrationScriptsPath));
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task ApplyMigrationsAsync()
    {
        var migrationFiles = Directory.GetFiles(_migrationScriptsPath, "*.sql").OrderBy(f => f);
        var appliedMigrations = await GetAppliedMigrationsAsync();

        foreach (var file in migrationFiles)
        {
            var scriptName = Path.GetFileName(file);
            if (!appliedMigrations.Contains(scriptName))
            {
                if (await TryAcquireLockAsync())
                {
                    try
                    {
                        var script = await File.ReadAllTextAsync(file);
                        using (var connection = CreateConnection())
                        {
                            await connection.ExecuteAsync(script);
                            await MarkMigrationAsAppliedAsync(scriptName);
                        }
                    }
                    finally
                    {
                        await ReleaseLockAsync();
                    }
                }
            }
        }
    }

    private async Task<bool> TryAcquireLockAsync()
    {
        const string acquireLockSql = @"
DECLARE @lockId INT = 1;
BEGIN TRANSACTION;
    IF NOT EXISTS (SELECT * FROM MigrationLock WITH (UPDLOCK, ROWLOCK) WHERE LockId = @lockId AND IsLocked = 1)
    BEGIN
        IF NOT EXISTS (SELECT * FROM MigrationLock WHERE LockId = @lockId)
        BEGIN
            INSERT INTO MigrationLock (LockId, IsLocked, LockedBy, LockAcquiredAt) VALUES (@lockId, 1, SYSTEM_USER, GETUTCDATE());
        END
        ELSE
        BEGIN
            UPDATE MigrationLock SET IsLocked = 1, LockedBy = SYSTEM_USER, LockAcquiredAt = GETUTCDATE() WHERE LockId = @lockId;
        END
        COMMIT;
        SELECT 1; -- Lock acquired
    END
    ELSE
    BEGIN
        ROLLBACK;
        SELECT 0; -- Lock not acquired
    END";

        using (var connection = CreateConnection())
        {
            var result = await connection.QuerySingleOrDefaultAsync<int>(acquireLockSql);
            return result == 1;
        }
    }

    private async Task ReleaseLockAsync()
    {
        const string releaseLockSql = @"
UPDATE MigrationLock SET IsLocked = 0, LockedBy = NULL, LockAcquiredAt = NULL WHERE LockId = 1";
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(releaseLockSql);
        }
    }

    private async Task<HashSet<string>> GetAppliedMigrationsAsync()
    {
        const string getAppliedMigrationsSql = @"
SELECT ScriptName FROM AppliedMigrations";
        using (var connection = CreateConnection())
        {
            var appliedMigrations = await connection.QueryAsync<string>(getAppliedMigrationsSql);
            return appliedMigrations.ToHashSet();
        }
    }

    private async Task MarkMigrationAsAppliedAsync(string scriptName)
    {
        const string markMigrationAsAppliedSql = @"
INSERT INTO AppliedMigrations (ScriptName, AppliedBy, AppliedAt) VALUES (@ScriptName, SYSTEM_USER, GETUTCDATE())";
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(markMigrationAsAppliedSql, new { ScriptName = scriptName });
        }
    }
}

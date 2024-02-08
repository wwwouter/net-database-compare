using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class MigrationService
{
    private readonly string _connectionString;
    private readonly string _migrationScriptsPath;

    public MigrationService(string connectionString, string migrationScriptsPath)
    {
        _connectionString = connectionString;
        _migrationScriptsPath = migrationScriptsPath;
    }

    public async Task ApplyMigrations()
    {
        await EnsureDatabaseStructure();

        var instanceId = Guid.NewGuid().ToString(); // Unique identifier for this instance
        var migrationFiles = Directory.GetFiles(_migrationScriptsPath, "*.sql").OrderBy(f => f);

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var appliedMigrations = await connection.QueryAsync<string>("SELECT ScriptName FROM AppliedMigrations");
            var appliedMigrationsSet = new HashSet<string>(appliedMigrations);

            foreach (var file in migrationFiles)
            {
                var scriptName = Path.GetFileName(file);
                if (!appliedMigrationsSet.Contains(scriptName))
                {
                    if (await TryAcquireLockAsync(connection, instanceId))
                    {
                        try
                        {
                            // Read SQL script from file
                            var script = await File.ReadAllTextAsync(file);

                            // Execute the script
                            await connection.ExecuteAsync(script);

                            // Log the migration as applied
                            var insertMigrationSql = "INSERT INTO AppliedMigrations (ScriptName, AppliedBy, AppliedAt) VALUES (@ScriptName, @AppliedBy, @AppliedAt)";
                            await connection.ExecuteAsync(insertMigrationSql, new { ScriptName = scriptName, AppliedBy = instanceId, AppliedAt = DateTime.UtcNow });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to apply migration script {scriptName}: {ex.Message}", ex);
                        }
                        finally
                        {
                            await ReleaseLockAsync(connection);
                        }
                    }
                }
            }
        }
    }

    private async Task<bool> TryAcquireLockAsync(SqlConnection connection, string instanceId)
    {
        const int lockId = 1; // Simplified for example
        var lockRecord = await connection.QueryFirstOrDefaultAsync<MigrationLock>("SELECT * FROM MigrationLock WHERE LockId = @LockId FOR UPDATE", new { LockId = lockId });

        if (lockRecord == null)
        {
            // Initialize lock record if not present
            await connection.ExecuteAsync("INSERT INTO MigrationLock (LockId, IsLocked, LockedBy, LockAcquiredAt) VALUES (@LockId, @IsLocked, @LockedBy, @LockAcquiredAt)",
                new { LockId = lockId, IsLocked = true, LockedBy = instanceId, LockAcquiredAt = DateTime.UtcNow });
            return true;
        }
        else if (!lockRecord.IsLocked)
        {
            // Acquire the lock
            await connection.ExecuteAsync("UPDATE MigrationLock SET IsLocked = @IsLocked, LockedBy = @LockedBy, LockAcquiredAt = @LockAcquiredAt WHERE LockId = @LockId",
                new { LockId = lockId, IsLocked = true, LockedBy = instanceId, LockAcquiredAt = DateTime.UtcNow });
            return true;
        }

        return false; // Lock is already held
    }

    private async Task ReleaseLockAsync(SqlConnection connection)
    {
        const int lockId = 1; // Simplified for example
        await connection.ExecuteAsync("UPDATE MigrationLock SET IsLocked = 0, LockedBy = NULL, LockAcquiredAt = NULL WHERE LockId = @LockId",
            new { LockId = lockId });
    }

    private async Task EnsureDatabaseStructure()
    {
        var createAppliedMigrationsTableSql = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppliedMigrations')
BEGIN
    CREATE TABLE AppliedMigrations (
        MigrationId INT IDENTITY(1,1) PRIMARY KEY,
        ScriptName VARCHAR(255) NOT NULL,
        AppliedBy VARCHAR(255) NOT NULL,
        AppliedAt DATETIME NOT NULL
    );
    CREATE NONCLUSTERED INDEX IX_AppliedMigrations_ScriptName ON AppliedMigrations (ScriptName);
END";

        var createMigrationLockTableSql = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MigrationLock')
BEGIN
    CREATE TABLE MigrationLock (
        LockId INT PRIMARY KEY,
        IsLocked BIT NOT NULL,
        LockedBy VARCHAR(255),
        LockAcquiredAt DATETIME
    );
END";

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var transaction = await connection.BeginTransactionAsync(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    await connection.ExecuteAsync(createAppliedMigrationsTableSql, transaction: transaction);
                    await connection.ExecuteAsync(createMigrationLockTableSql, transaction: transaction);
                    await transaction.CommitAsync();
                }
                catch (SqlException ex)
                {
                    // Assuming the error is due to the table already existing, which is safe to ignore.
                    // If using a more specific error code check, adjust the condition accordingly.
                    await transaction.RollbackAsync();
                }
            }
        }
    }



}

public class MigrationLock
{
    public int LockId { get; set; }
    public bool IsLocked { get; set; }
    public string LockedBy { get; set; }
    public DateTime? LockAcquiredAt { get; set; }
}

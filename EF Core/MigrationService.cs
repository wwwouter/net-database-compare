using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class MigrationService
{
    private readonly AppDbContext _context;
    private readonly string _migrationScriptsPath;

    public MigrationService(AppDbContext context, string migrationScriptsPath)
    {
        _context = context;
        _migrationScriptsPath = migrationScriptsPath;
    }
    public async Task ApplyMigrations()
    {
        var instanceId = Guid.NewGuid().ToString(); // Unique identifier for this instance
        var migrationFiles = Directory.GetFiles(_migrationScriptsPath, "*.sql").OrderBy(f => f);
        var migrationFileNames = migrationFiles.Select(Path.GetFileName).ToHashSet();

        // Retrieve all migrations that have been applied from the database
        var appliedMigrations = await _context.AppliedMigrations.Select(m => m.ScriptName).ToListAsync();

        // Check if any applied migrations are missing from the disk
        var missingMigrations = appliedMigrations.Except(migrationFileNames).ToList();
        if (missingMigrations.Any())
        {
            throw new InvalidOperationException($"The following migrations have been applied to the database but are missing from disk: {string.Join(", ", missingMigrations)}");
        }

        foreach (var file in migrationFiles)
        {
            var scriptName = Path.GetFileName(file);
            var alreadyApplied = appliedMigrations.Contains(scriptName);
            if (!alreadyApplied)
            {
                if (await TryAcquireLockAsync(instanceId))
                {
                    try
                    {
                        // Start a transaction for each script execution
                        using (var transaction = await _context.Database.BeginTransactionAsync())
                        {
                            var script = await File.ReadAllTextAsync(file);
                            await _context.Database.ExecuteSqlRawAsync(script);

                            // Log the migration as applied
                            await _context.AppliedMigrations.AddAsync(new AppliedMigration
                            {
                                ScriptName = scriptName,
                                AppliedBy = instanceId,
                                AppliedAt = DateTime.UtcNow
                            });
                            await _context.SaveChangesAsync();

                            // Commit the transaction if script executes successfully
                            await transaction.CommitAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback is handled automatically by the using statement if an exception occurs

                        // Release the lock in case of failure
                        await ReleaseLockAsync();

                        // Rethrow the exception to stop the application
                        throw new Exception($"Failed to apply migration script {scriptName}: {ex.Message}", ex);
                    }
                    finally
                    {
                        // Ensure the lock is released after attempting to apply the migration
                        await ReleaseLockAsync();
                    }
                }
            }
        }
    }


    public async Task<bool> TryAcquireLockAsync(string instanceId)
    {
        const int lockId = 1; // Assuming a single lock record with a known ID for simplicity

        // Start a transaction
        using (var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable))
        {
            try
            {
                // Attempt to acquire the lock using UPDLOCK and ROWLOCK hints to prevent other transactions from modifying or reading the row
                var lockRecord = await _context.MigrationLocks
                    .FromSqlInterpolated($"SELECT * FROM MigrationLock WITH (UPDLOCK, ROWLOCK) WHERE LockId = {lockId}")
                    .FirstOrDefaultAsync();

                if (lockRecord == null)
                {
                    // Initialize lock record if not present
                    lockRecord = new MigrationLock { LockId = lockId, IsLocked = false, LockedBy = null, LockAcquiredAt = null };
                    _context.MigrationLocks.Add(lockRecord);
                    await _context.SaveChangesAsync();
                }

                if (!lockRecord.IsLocked)
                {
                    // Acquire the lock
                    lockRecord.IsLocked = true;
                    lockRecord.LockedBy = instanceId;
                    lockRecord.LockAcquiredAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // Commit the transaction to release the lock
                    await transaction.CommitAsync();

                    return true; // Successfully acquired the lock
                }

                // Lock is already held by another instance, do not commit the transaction
                return false;
            }
            catch
            {
                // Ensure the transaction is rolled back if an exception occurs
                await transaction.RollbackAsync();
                throw;
            }
        }
    }



    public async Task ReleaseLockAsync()
    {
        const int lockId = 1; // Assuming you're working with a single, known lock record

        // It's important to handle this operation as a transaction to ensure atomicity
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var lockRecord = await _context.MigrationLocks.FindAsync(lockId);
                if (lockRecord != null && lockRecord.IsLocked)
                {
                    // Release the lock
                    lockRecord.IsLocked = false;
                    lockRecord.LockedBy = null;
                    lockRecord.LockAcquiredAt = null;
                    await _context.SaveChangesAsync();

                    // Commit the transaction to finalize the lock release
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                // Roll back the transaction if there's an error
                await transaction.RollbackAsync();
                throw; // Re-throw the exception to be handled by the caller
            }
        }
    }


}


I'm writing a repository with EF Core to compare different data access packages. Write next missing method. Keep DTO nullable values nullable. If not clear, just assume a query, based on the description. MS SQL Server is the database.


Can you create the necessary files to Define the Entity Classes (in full) for this schema for ef core 8 and .NET 8 (used with an Entity Framework Core DbContext)? Add Data Annotations if relevant.
Use the correct type for Customers.GeographicLocation.  Projects.Status and Projects.Priority should be enums.


Step 1: 
Step 2: Configure DbContext
Step 3: Implement Repository


- EF COre
    - does validation by annotations work?
        - double check other packages







Where to call this? It should fail fast. So if there is an error in a sql file, it should rollback and stop the application. ```using Microsoft.EntityFrameworkCore;
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
        var migrationFiles = Directory.GetFiles("Path\\To\\Your\\Scripts\\Folder", "*.sql").OrderBy(f => f);
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
                // Acquire a lock as previously described to ensure only one instance applies this migration
                if (await TryAcquireLockAsync(instanceId))
                {
                    var script = await File.ReadAllTextAsync(file);
                    await _context.Database.ExecuteSqlRawAsync(script);
                    await _context.AppliedMigrations.AddAsync(new AppliedMigration
                    {
                        ScriptName = scriptName,
                        AppliedBy = instanceId,
                        AppliedAt = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();

                    // Release the lock
                    await ReleaseLockAsync();
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
``` Also the filepath should be configurable
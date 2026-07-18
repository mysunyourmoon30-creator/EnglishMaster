using EnglishMaster.Infrastructure.Persistence;
using EnglishMaster.Infrastructure.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace EnglishMaster.IntegrationTests.Persistence;

/// <summary>
/// Unlike <see cref="EnglishMasterApiFactory"/> (EF Core InMemory + EnsureCreated), this fixture
/// points at a real LocalDB database and runs actual EF Core migrations, so it can catch
/// migration-only defects (FK cascade-path errors, tables missing from migration history) that
/// the InMemory provider structurally cannot exercise.
/// </summary>
public sealed class RelationalEnglishMasterApiFactory : WebApplicationFactory<Program>
{
    private readonly string databaseName = $"EnglishMaster-IntegrationTest-{Guid.NewGuid():N}";

    private string MasterConnectionString =>
        "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";

    private string DatabaseConnectionString =>
        $"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:InitialSuperAdmin:Email"] = "superadmin@englishmaster.migrationtest",
                ["Auth:InitialSuperAdmin:Password"] = "TestPassword1",
                ["EmailDeliveryWorker:Enabled"] = "false"
            });
        });
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<EnglishMasterDbContext>();
            services.RemoveAll<DbContextOptions<EnglishMasterDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<EnglishMasterDbContext>>();

            services.AddDbContext<EnglishMasterDbContext>(options =>
                options.UseSqlServer(DatabaseConnectionString));
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Path.GetTempPath(), databaseName)))
                .SetApplicationName(databaseName);

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            // Real MigrateAsync() against a real relational provider, from a genuinely empty
            // database - this is what SecuritySeeder.SeedSecurityAsync's IsRelational() branch
            // does in production, and what the InMemory-based EnglishMasterApiFactory can never
            // exercise, since Database.IsRelational() is false for the InMemory provider.
            SecuritySeeder.SeedSecurityAsync(scope.ServiceProvider, configuration).GetAwaiter().GetResult();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DropDatabase();
        }

        base.Dispose(disposing);
    }

    private void DropDatabase()
    {
        using var connection = new SqlConnection(MasterConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            $"IF DB_ID('{databaseName}') IS NOT NULL BEGIN " +
            $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
            $"DROP DATABASE [{databaseName}]; END";
        command.ExecuteNonQuery();
    }
}

using EnglishMaster.Infrastructure.Persistence;
using EnglishMaster.Infrastructure.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace EnglishMaster.IntegrationTests;

public sealed class EnglishMasterApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:InitialSuperAdmin:Email"] = "superadmin@englishmaster.test",
                ["Auth:InitialSuperAdmin:Password"] = "TestPassword1"
            });
        });
        builder.ConfigureServices(services =>
        {
            var databaseName = $"EnglishMaster-{Guid.NewGuid()}";

            services.RemoveAll<EnglishMasterDbContext>();
            services.RemoveAll<DbContextOptions<EnglishMasterDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<EnglishMasterDbContext>>();

            services.AddDbContext<EnglishMasterDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Path.GetTempPath(), databaseName)))
                .SetApplicationName(databaseName);

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
            dbContext.Database.EnsureCreated();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            SecuritySeeder.SeedSecurityAsync(scope.ServiceProvider, configuration).GetAwaiter().GetResult();
        });
    }
}

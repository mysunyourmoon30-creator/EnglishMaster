using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.Infrastructure.Security;

public static class SecuritySeeder
{
    public static async Task SeedSecurityAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        if (dbContext.Database.IsRelational())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        var service = (EfSecurityService)scope.ServiceProvider.GetRequiredService<EnglishMaster.Application.Features.Security.ISecurityService>();
        await service.SeedDefaultsAsync(
            configuration["Auth:InitialSuperAdmin:Email"],
            configuration["Auth:InitialSuperAdmin:Password"],
            cancellationToken);

        if (configuration.GetValue<bool>("DevelopmentSeed:Enabled"))
        {
            var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
            var developmentSeedDataSeeder = new DevelopmentSeedDataSeeder(dbContext, timeProvider);
            await developmentSeedDataSeeder.SeedAsync(cancellationToken);
        }
    }
}

using EnglishMaster.Contracts.Analytics;

namespace EnglishMaster.Web.Services.Analytics;

public interface IAnalyticsApiClient
{
    Task<AdminAnalyticsOverviewDto> GetAdminOverviewAsync(CancellationToken cancellationToken);
}

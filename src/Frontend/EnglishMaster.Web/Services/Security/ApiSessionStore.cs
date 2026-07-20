using System.Collections.Concurrent;

namespace EnglishMaster.Web.Services.Security;

internal interface IApiSessionStore
{
    string Store(string apiCookie);

    string? Get(string sessionId);

    void Remove(string sessionId);
}

internal sealed class InMemoryApiSessionStore(TimeProvider timeProvider) : IApiSessionStore
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(8);
    private readonly ConcurrentDictionary<string, ApiSession> sessions = new(StringComparer.Ordinal);

    public string Store(string apiCookie)
    {
        RemoveExpiredSessions();
        var sessionId = Guid.NewGuid().ToString("N");
        sessions[sessionId] = new ApiSession(apiCookie, timeProvider.GetUtcNow().Add(SessionLifetime));
        return sessionId;
    }

    public string? Get(string sessionId)
    {
        if (!sessions.TryGetValue(sessionId, out var session))
        {
            return null;
        }

        if (session.ExpiresAt <= timeProvider.GetUtcNow())
        {
            sessions.TryRemove(sessionId, out _);
            return null;
        }

        return session.ApiCookie;
    }

    public void Remove(string sessionId) =>
        sessions.TryRemove(sessionId, out _);

    private void RemoveExpiredSessions()
    {
        var now = timeProvider.GetUtcNow();
        foreach (var item in sessions.Where(item => item.Value.ExpiresAt <= now))
        {
            sessions.TryRemove(item.Key, out _);
        }
    }

    private sealed record ApiSession(string ApiCookie, DateTimeOffset ExpiresAt);
}

using System.Collections.Concurrent;

namespace EnglishMaster.Api.Security;

internal sealed class LoginAttemptTracker(TimeProvider timeProvider)
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
    private readonly ConcurrentDictionary<string, LoginAttemptState> attempts = new(StringComparer.Ordinal);

    public bool IsLockedOut(string? email, string remoteAddress, out TimeSpan retryAfter)
    {
        var key = BuildKey(email, remoteAddress);
        retryAfter = TimeSpan.Zero;
        if (!attempts.TryGetValue(key, out var state))
        {
            return false;
        }

        var now = timeProvider.GetUtcNow();
        if (state.LockedUntil is null || state.LockedUntil <= now)
        {
            attempts.TryRemove(key, out _);
            return false;
        }

        retryAfter = state.LockedUntil.Value - now;
        return true;
    }

    public void RecordFailure(string? email, string remoteAddress)
    {
        var key = BuildKey(email, remoteAddress);
        var now = timeProvider.GetUtcNow();
        attempts.AddOrUpdate(
            key,
            _ => new LoginAttemptState(1, null, now),
            (_, current) =>
            {
                var failedAttempts = current.LastAttemptAt.Add(LockoutDuration) <= now
                    ? 1
                    : current.FailedAttempts + 1;
                var lockedUntil = failedAttempts >= MaxFailedAttempts
                    ? now.Add(LockoutDuration)
                    : current.LockedUntil;
                return new LoginAttemptState(failedAttempts, lockedUntil, now);
            });
    }

    public void RecordSuccess(string? email, string remoteAddress) =>
        attempts.TryRemove(BuildKey(email, remoteAddress), out _);

    private static string BuildKey(string? email, string remoteAddress) =>
        $"{remoteAddress.Trim()}:{(email ?? string.Empty).Trim().ToUpperInvariant()}";

    private sealed record LoginAttemptState(int FailedAttempts, DateTimeOffset? LockedUntil, DateTimeOffset LastAttemptAt);
}

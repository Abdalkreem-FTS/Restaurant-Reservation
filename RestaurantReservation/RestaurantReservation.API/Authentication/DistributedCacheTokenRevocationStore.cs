using Microsoft.Extensions.Caching.Distributed;

namespace RestaurantReservation.API.Authentication;

/// <summary>
/// Black List
/// </summary>
public sealed class DistributedCacheTokenRevocationStore(
    IDistributedCache cache,
    ILogger<DistributedCacheTokenRevocationStore> logger) : ITokenRevocationStore
{
    private static readonly TimeSpan ExpiryBuffer = TimeSpan.FromMinutes(1);

    private static readonly byte[] Revoked = [.. "1"u8];

    public async Task<bool> RevokeAsync(string tokenId, DateTimeOffset expiresAt, CancellationToken cancellationToken = default)
    {
        var remaining = expiresAt - DateTimeOffset.UtcNow;

        if (remaining <= TimeSpan.Zero)
        {
            return true;
        }

        try
        {
            await cache.SetAsync(
                Key(tokenId),
                Revoked,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = remaining + ExpiryBuffer },
                cancellationToken);

            return true;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Could not record the revocation of token {TokenId}", tokenId);

            return false;
        }
    }

    public async Task<bool> IsRevokedAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await cache.GetAsync(Key(tokenId), cancellationToken) is not null;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "Could not check whether token {TokenId} was revoked; treating it as valid", tokenId);

            return false;
        }
    }

    private static string Key(string tokenId) => $"revoked-token:{tokenId}";
}

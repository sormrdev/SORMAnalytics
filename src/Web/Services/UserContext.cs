using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using SORMAnalytics.Application.Common.Interfaces;
using SORMAnalytics.Infrastructure.Data;

using Web.Infrastructure;

namespace Web.Services;

public class UserContext(
    IHttpContextAccessor httpContextAccessor,
    IApplicationDbContext dbContext,
    IMemoryCache memoryCache)
{
    private const string CacheKeyPrefix = "users:id:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    public async Task<string?> GetUserIdAsync(CancellationToken cancellationToken = default)
    {
        string? identityId = httpContextAccessor.HttpContext?.User.GetIdentityId();

        if (string.IsNullOrEmpty(identityId))
            return null;

        string cacheKey = CacheKeyPrefix + identityId;

        string? userId = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
           entry.SetSlidingExpiration(CacheDuration);

           string? userId = await dbContext.Users
                .Where(u => u.IdentityId == identityId)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return userId;
        });

        return userId;
    }
}

using Devpull.Controllers;
using Devpull.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Devpull.Users;

public class TokenRepository
{
    private readonly AppDbContext _db;

    public TokenRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<RefreshToken?> GetRefreshToken(
        string refreshToken,
        CancellationToken cancellationToken
    )
    {
        return await _db.RefreshTokens.SingleOrDefaultAsync(
            t => t.Token == refreshToken || t.OldToken == refreshToken,
            cancellationToken
        );
    }

    public async Task<int> InsertRefreshToken(
        string userId,
        string deviceFingerprint,
        string refreshToken,
        DateTime expiryDate,
        bool rememberMe,
        CancellationToken cancellationToken
    )
    {
        var appUserToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            DeviceFingerprint = deviceFingerprint,
            UserId = userId,
            Token = refreshToken,
            ExpiryDate = expiryDate,
            RememberMe = rememberMe,
            OldToken = null,
            RefreshDate = null
        };
        _db.RefreshTokens.Add(appUserToken);
        return await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> UpdateRefreshToken(
        RefreshToken refreshToken,
        CancellationToken cancellationToken
    )
    {
        _db.RefreshTokens.Update(refreshToken);
        return await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<int> DeleteRefreshToken(
        RefreshToken refreshToken,
        CancellationToken cancellationToken
    )
    {
        _db.RefreshTokens.Remove(refreshToken);
        return _db.SaveChangesAsync(cancellationToken);
    }

    public Task<int> DeleteRefreshToken(
        string userId,
        string deviceFingerprint,
        CancellationToken cancellationToken
    )
    {
        return _db.RefreshTokens
            .Where(t => t.UserId == userId && t.DeviceFingerprint == deviceFingerprint)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task<int> DeleteAllRefreshTokens(string userId, CancellationToken cancellationToken)
    {
        return _db.RefreshTokens
            .Where(t => t.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}

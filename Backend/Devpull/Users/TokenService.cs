using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Devpull.Controllers;
using Devpull.DbModels;
using Microsoft.IdentityModel.Tokens;

namespace Devpull.Users;

public interface ITokenService
{
    Task<RefreshToken?> GetRefreshToken(string refreshToken, CancellationToken cancellationToken);

    Task<int> InsertRefreshToken(
        string userId,
        string deviceFingerprint,
        string refreshToken,
        bool rememberMe,
        CancellationToken cancellationToken
    );

    Task<int> UpdateRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken);

    string GenerateJwtToken(AppUser user);
    string GenerateRefreshToken();

    Task<int> DeleteRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken);

    Task<int> DeleteRefreshToken(
        string userId,
        string deviceFingerprint,
        CancellationToken cancellationToken
    );

    Task<int> DeleteAllRefreshTokens(string userId, CancellationToken cancellationToken);
}

public class TokenService : ITokenService
{
    private readonly Devpull.Common.AppConfig _config;
    private readonly TokenRepository _tokenRepository;

    public TokenService(Devpull.Common.AppConfig config, TokenRepository tokenRepository)
    {
        _config = config;
        _tokenRepository = tokenRepository;
    }

    public Task<RefreshToken?> GetRefreshToken(
        string refreshToken,
        CancellationToken cancellationToken
    )
    {
        return _tokenRepository.GetRefreshToken(refreshToken, cancellationToken);
    }

    public Task<int> InsertRefreshToken(
        string userId,
        string deviceFingerprint,
        string refreshToken,
        bool rememberMe,
        CancellationToken cancellationToken
    )
    {
        var expireConfig = rememberMe
            ? _config.Jwt.RefreshTokenExpirationInSeconds
            : _config.Jwt.ShortRefreshTokenExpirationInSeconds;
        return _tokenRepository.InsertRefreshToken(
            userId,
            deviceFingerprint,
            refreshToken,
            DateTime.UtcNow.AddSeconds(expireConfig),
            rememberMe,
            cancellationToken
        );
    }

    public Task<int> UpdateRefreshToken(
        RefreshToken refreshToken,
        CancellationToken cancellationToken
    )
    {
        var expireConfig = refreshToken.RememberMe
            ? _config.Jwt.RefreshTokenExpirationInSeconds
            : _config.Jwt.ShortRefreshTokenExpirationInSeconds;
        refreshToken.ExpiryDate = DateTime.UtcNow.AddSeconds(expireConfig);
        return _tokenRepository.UpdateRefreshToken(refreshToken, cancellationToken);
    }

    public string GenerateJwtToken(AppUser user)
    {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Jwt.Key!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

        var token = new JwtSecurityToken(
            issuer: _config.Jwt.Issuer,
            audience: _config.Jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(
                _config.Jwt.AccessTokenExpirationInSeconds
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Генерация случайного Refresh Token
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public Task<int> DeleteRefreshToken(
        RefreshToken refreshToken,
        CancellationToken cancellationToken
    )
    {
        return _tokenRepository.DeleteRefreshToken(refreshToken, cancellationToken);
    }

    public Task<int> DeleteRefreshToken(
        string userId,
        string deviceFingerprint,
        CancellationToken cancellationToken
    )
    {
        return _tokenRepository.DeleteRefreshToken(userId, deviceFingerprint, cancellationToken);
    }

    public Task<int> DeleteAllRefreshTokens(string userId, CancellationToken cancellationToken)
    {
        return _tokenRepository.DeleteAllRefreshTokens(userId, cancellationToken);
    }
}

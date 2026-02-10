using System.IdentityModel.Tokens.Jwt;
using Devpull.Controllers;

namespace Devpull.Course;

public class AuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsUserAuthenticatedOrThrow()
    {
        var isAuthenticated =
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        if (isAuthenticated)
        {
            return true;
        }
        throw new AuthenticationException("Ошибка аутентификации");
    }

    public string GetUserIdOrThrow()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? throw new AuthenticationException("Ошибка аутентификации");
    }

    public string GetUserEmailOrThrow()
    {
        return _httpContextAccessor.HttpContext
                ?.User?.FindFirst(JwtRegisteredClaimNames.Email)
                ?.Value ?? throw new AuthenticationException("Ошибка аутентификации");
    }

    public string? GetUserId()
    {
        return _httpContextAccessor.HttpContext
            ?.User?.FindFirst(JwtRegisteredClaimNames.Sub)
            ?.Value;
    }

    public string? GetRequestId()
    {
        return _httpContextAccessor.HttpContext?.TraceIdentifier;
    }
}

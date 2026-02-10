using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Devpull.Controllers;
using Devpull.Controllers.Models;
using Devpull.Course;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Devpull.Users;

public class UserService
{
    private readonly IUserManagerService _userManager;
    private readonly Devpull.Common.AppConfig _config;
    private readonly LoginModelValidator _loginModelValidator;
    private readonly RegisterModelValidator _registerModelValidator;
    private readonly IEmailSender _emailSender;
    private readonly ConfirmEmailModelValidator _confirmEmailModelValidator;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly ICodeGenerator _codeGenerator;
    private readonly LogoutModelValidator _logoutModelValidator;
    private readonly RefreshTokenModelValidator _refreshTokenModelValidator;
    private readonly ForgotPasswordModelValidator _forgotPasswordModelValidator;
    private readonly ResetPasswordModelValidator _resetPasswordModelValidator;

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _refreshLocks = new();

    public UserService(
        IUserManagerService userManager,
    Devpull.Common.AppConfig config,
        LoginModelValidator loginModelValidator,
        RegisterModelValidator registerModelValidator,
        IEmailSender emailSender,
        ConfirmEmailModelValidator confirmEmailModelValidator,
        SignInManager<AppUser> signInManager,
        AuthService authService,
        ITokenService tokenService,
        ICodeGenerator codeGenerator,
        LogoutModelValidator logoutModelValidator,
        RefreshTokenModelValidator refreshTokenModelValidator,
        ForgotPasswordModelValidator forgotPasswordModelValidator,
        ResetPasswordModelValidator resetPasswordModelValidator
    )
    {
        _userManager = userManager;
    _config = config;
        _loginModelValidator = loginModelValidator;
        _registerModelValidator = registerModelValidator;
        _emailSender = emailSender;
        _confirmEmailModelValidator = confirmEmailModelValidator;
        _signInManager = signInManager;
        _authService = authService;
        _tokenService = tokenService;
        _codeGenerator = codeGenerator;
        _logoutModelValidator = logoutModelValidator;
        _refreshTokenModelValidator = refreshTokenModelValidator;
        _forgotPasswordModelValidator = forgotPasswordModelValidator;
        _resetPasswordModelValidator = resetPasswordModelValidator;
    }

    public async Task<bool> Register(RegisterModel model, CancellationToken cancellationToken)
    {
        // кейсы
        // 1. нет почты - создаем
        // 2. есть почта и запись подтверждена - ошибка валидации
        // 3. есть почта и запись НЕ подтверждена - обновить код
        await _registerModelValidator.Validate(model);

        var code = _codeGenerator.Generate();

        var user = await _userManager.FindByEmailMaybeAsync(model.Email);
        IdentityResult? result;
        var codeExpiry = DateTime.UtcNow.AddSeconds(_config.User.ValidCodeTimeInSeconds);
        if (user is null)
        {
            var newUser = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmationCode = code,
                EmailConfirmationCodeExpiry = codeExpiry,
            };
            result = await _userManager.CreateAsync(newUser, model.Password);
        }
        else
        {
            user.EmailConfirmationCode = code;
            user.EmailConfirmationCodeExpiry = codeExpiry;

            result = await _userManager.UpdateAsync(user);
        }

        if (result.Succeeded)
        {
            await _emailSender.SendEmailAsync(
                model.Email,
                "Подтверждение регистрации на devpull.courses",
                $"Ваш код для подтверждения регистрации: <b>{code}</b>"
            );
            return true;
        }

        throw new Exception(
            string.Join(", ", result.Errors.Select(e => $"{e.Description} ({e.Code})"))
        );
    }

    public async Task<TokensModel> Login(LoginModel model, CancellationToken cancellationToken)
    {
        await _loginModelValidator.Validate(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        var tokens = GenerateTokens(user);
        await _tokenService.DeleteRefreshToken(user.Id, model.DeviceFingerprint, cancellationToken);
        await _tokenService.InsertRefreshToken(
            user.Id,
            model.DeviceFingerprint,
            tokens.RefreshToken,
            model.RememberMe!.Value,
            cancellationToken
        );
        return tokens;
    }

    public async Task<TokensModel> RefreshTokens(
        RefreshTokenModel model,
        CancellationToken cancellationToken
    )
    {
        // получаем семафор для конкретного refreshToken
        var semaphore = _refreshLocks.GetOrAdd(model.RefreshToken, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);
        try
        {
            // === Заблокированный участок ===
            await _refreshTokenModelValidator.Validate(model);
            Console.WriteLine($"Запрос на refresh: {model.RefreshToken}, {DateTime.UtcNow}");

            var refreshToken = await _tokenService.GetRefreshToken(
                model.RefreshToken,
                cancellationToken
            );

            if (refreshToken is null)
            {
                throw new AuthenticationException("Невалидный refresh token");
            }

            if (refreshToken.ExpiryDate < DateTime.UtcNow)
            {
                await _tokenService.DeleteRefreshToken(refreshToken, cancellationToken);
                throw new AuthenticationException("Срок действия рефреш токена истек");
            }

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            var refreshTimeout = _config.Jwt.RefreshTimeoutInSeconds;

            // если пришел старый токен - это валидный кейс при гонке при быстром открытии вкладок
            if (refreshToken.OldToken == model.RefreshToken && refreshToken.RefreshDate.HasValue)
            {
                if (refreshToken.RefreshDate.Value.AddSeconds(refreshTimeout) > DateTime.UtcNow)
                {
                    var token = _tokenService.GenerateJwtToken(user);
                    Console.WriteLine(
                        $"Гонка, пришел старый токен, отдаем актуальный: {refreshToken.Token}"
                    );
                    return new TokensModel { Token = token, RefreshToken = refreshToken.Token };
                }

                throw new AuthenticationException("Невалидный refresh token");
            }

            // если пришел запрос на refresh и прошло мало времени - отдаем актуальный RefreshToken, это валидный кейс при гонке при быстром открытии вкладок
            Console.WriteLine($"Now: {DateTime.UtcNow}, RefreshDate: {refreshToken.RefreshDate}");
            if (
                refreshToken.RefreshDate.HasValue
                && refreshToken.RefreshDate.Value.AddSeconds(refreshTimeout) > DateTime.UtcNow
            )
            {
                var token = _tokenService.GenerateJwtToken(user);
                Console.WriteLine($"Гонка, пришел новый токен, не рефрешим: {refreshToken.Token}");
                return new TokensModel { Token = token, RefreshToken = refreshToken.Token };
            }

            var tokens = GenerateTokens(user);
            refreshToken.OldToken = refreshToken.Token;
            refreshToken.RefreshDate = DateTime.UtcNow;
            refreshToken.Token = tokens.RefreshToken;
            await _tokenService.UpdateRefreshToken(refreshToken, cancellationToken);
            Console.WriteLine($"New refresh token: {refreshToken.Token}");
            return tokens;
        }
        finally
        {
            // обязательно освободить блокировку
            semaphore.Release();

            // удалить семафор, если он больше не используется (опционально)
            if (semaphore.CurrentCount == 1)
            {
                _refreshLocks.TryRemove(model.RefreshToken, out _);
            }
        }
    }

    public async Task<TokensModel> ConfirmEmail(
        ConfirmEmailModel model,
        CancellationToken cancellationToken
    )
    {
        await _confirmEmailModelValidator.Validate(model);

        var user = await _userManager.FindByEmailAsync(model.Email);

        user!.EmailConfirmed = true;
        user.EmailConfirmationCode = null; // Clear the stored code
        user.EmailConfirmationCodeExpiry = null;
        await _userManager.UpdateAsync(user);

        var tokens = GenerateTokens(user);
        await _tokenService.InsertRefreshToken(
            user.Id,
            model.DeviceFingerprint,
            tokens.RefreshToken,
            rememberMe: true,
            cancellationToken
        );
        return tokens;
    }

    public async Task<int> Logout(LogoutModel model, CancellationToken cancellationToken)
    {
        await _logoutModelValidator.Validate(model);

        var userId = _authService.GetUserIdOrThrow();
        var refreshToken = await _tokenService.GetRefreshToken(
            model.RefreshToken,
            cancellationToken
        );
        if (refreshToken is not null)
        {
            return await _tokenService.DeleteRefreshToken(refreshToken, cancellationToken);
        }

        return 0;
    }

    private TokensModel GenerateTokens(AppUser user)
    {
        var token = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        return new TokensModel { Token = token, RefreshToken = refreshToken };
    }

    public Task<bool> Test()
    {
        return Task.FromResult(_authService.IsUserAuthenticatedOrThrow());
    }

    public async Task<bool> ForgotPassword(
        ForgotPasswordModel model,
        CancellationToken cancellationToken
    )
    {
        await _forgotPasswordModelValidator.Validate(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        var code = _codeGenerator.Generate();
        var codeExpiry = DateTime.UtcNow.AddSeconds(_config.User.ValidCodeTimeInSeconds);
        user.EmailConfirmationCode = code;
        user.EmailConfirmationCodeExpiry = codeExpiry;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            await _emailSender.SendEmailAsync(
                model.Email,
                "Email Confirmation Code",
                $"Your confirmation code is: {code}"
            );
            return true;
        }

        throw new Exception(
            string.Join(", ", result.Errors.Select(e => $"{e.Description} ({e.Code})"))
        );
    }

    public async Task<TokensModel> ResetPassword(
        ResetPasswordModel model,
        CancellationToken cancellationToken
    )
    {
        await _resetPasswordModelValidator.Validate(model);

        var user = await _userManager.FindByEmailAsync(model.Email);

        await _tokenService.DeleteAllRefreshTokens(user.Id, cancellationToken);

        await _userManager.ResetPassword(user, model.Password);

        // login
        user!.EmailConfirmed = true;
        user.EmailConfirmationCode = null;
        user.EmailConfirmationCodeExpiry = null;
        await _userManager.UpdateAsync(user);

        var tokens = GenerateTokens(user);
        await _tokenService.InsertRefreshToken(
            user.Id,
            model.DeviceFingerprint,
            tokens.RefreshToken,
            rememberMe: true,
            cancellationToken
        );
        return tokens;
    }

    public async Task<UserInfo> GetUserInfo(CancellationToken cancellationToken)
    {
        var userId = _authService.GetUserIdOrThrow();
        var user = await _userManager.FindByIdAsync(userId);
        var userInfo = new UserInfo
        {
            Email = user.Email,
            Subscriptions = user.Subscriptions
                .Select(
                    s =>
                        new SubscriptionInfo
                        {
                            Amount = s.Payment.Amount,
                            StartDate = s.StartDate,
                            EndDate = s.EndDate,
                            Type = s.Payment.Type
                        }
                )
                .OrderByDescending(s => s.EndDate)
                .ToArray()
        };
        return userInfo;
    }
}

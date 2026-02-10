using Devpull.Controllers;
using Devpull.Controllers.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationTests.Users;

public class RefreshTokenTests : TestBase
{
    public RefreshTokenTests(DatabaseFixture fixture)
        : base(fixture) { }

    [Fact]
    public async Task invalid_model_should_be_validation_errors()
    {
        var refreshTokenModel = new RefreshTokenModel { RefreshToken = "" };
        var refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );

        refreshTokenResult.Status.Should().Be(Status.ValidationFailed);
        refreshTokenResult.ValidationErrors.Should().HaveCount(1);
        refreshTokenResult.ValidationErrors
            .Should()
            .ContainKey("refreshToken")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Refresh Token обязателен");
    }

    [Fact]
    public async Task invalid_token_should_be_not_found_error()
    {
        var refreshTokenModel = new RefreshTokenModel { RefreshToken = "42" };
        var refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );

        refreshTokenResult.Status.Should().Be(Status.AuthenticationFailed);
    }

    [Fact]
    public async Task valid_token_and_device_should_be_success()
    {
        // регистрация
        var email = $"{Guid.NewGuid()}@mail.ru";
        var registerModel = new RegisterModel
        {
            Email = email,
            Password = "42",
            ConfirmPassword = "42"
        };
        var registerResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/register",
            registerModel
        );

        registerResult.Status.Should().Be(Status.Success);
        registerResult.Data.Should().Be(true);

        // правильный код
        var user = await _db.Users.SingleAsync(
            u => u.Email == email,
            cancellationToken: Xunit.TestContext.Current.CancellationToken
        );
        user.EmailConfirmationCode.Should().NotBeNullOrEmpty();

        var confirmEmailModel = new ConfirmEmailModel
        {
            Code = user.EmailConfirmationCode,
            Email = email,
            DeviceFingerprint = "42"
        };
        var confirmEmailResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/confirm-email",
            confirmEmailModel
        );

        confirmEmailResult.Status.Should().Be(Status.Success);
        confirmEmailResult.Data.Token.Should().NotBeNullOrEmpty();
        confirmEmailResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        // рефреш токенов
        var refreshTokenModel = new RefreshTokenModel
        {
            RefreshToken = confirmEmailResult.Data.RefreshToken
        };
        var refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult.Status.Should().Be(Status.Success);
        refreshTokenResult.Data.Token.Should().NotBeNullOrEmpty();
        refreshTokenResult.Data.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task refresh_token_expired_should_be_error()
    {
        // регистрация
        var email = $"{Guid.NewGuid()}@mail.ru";
        var registerModel = new RegisterModel
        {
            Email = email,
            Password = "42",
            ConfirmPassword = "42"
        };
        var registerResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/register",
            registerModel
        );

        registerResult.Status.Should().Be(Status.Success);
        registerResult.Data.Should().Be(true);

        // правильный код
        var user = await _db.Users.SingleAsync(
            u => u.Email == email,
            cancellationToken: Xunit.TestContext.Current.CancellationToken
        );
        user.EmailConfirmationCode.Should().NotBeNullOrEmpty();

        var confirmEmailModel = new ConfirmEmailModel
        {
            Code = user.EmailConfirmationCode,
            Email = email,
            DeviceFingerprint = "42"
        };
        var confirmEmailResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/confirm-email",
            confirmEmailModel
        );

        confirmEmailResult.Status.Should().Be(Status.Success);
        confirmEmailResult.Data.Token.Should().NotBeNullOrEmpty();
        confirmEmailResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        await Task.Delay(
            int.Parse(_config["Jwt:RefreshTokenExpirationInSeconds"]!) * 1000 + 10,
            Xunit.TestContext.Current.CancellationToken
        );
        // рефреш токенов
        var refreshTokenModel = new RefreshTokenModel
        {
            RefreshToken = confirmEmailResult.Data.RefreshToken
        };
        var refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult.Status.Should().Be(Status.AuthenticationFailed);
        refreshTokenResult.Error.Should().Be("Срок действия рефреш токена истек");

        // проверка, что токен удалился
        refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult.Status.Should().Be(Status.AuthenticationFailed);
    }

    [Fact]
    public async Task refresh_token_life_cycle()
    {
        // регистрация
        var email = $"{Guid.NewGuid()}@mail.ru";
        var registerModel = new RegisterModel
        {
            Email = email,
            Password = "42",
            ConfirmPassword = "42"
        };
        var registerResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/register",
            registerModel
        );

        registerResult.Status.Should().Be(Status.Success);
        registerResult.Data.Should().Be(true);

        // правильный код
        var user = await _db.Users.SingleAsync(
            u => u.Email == email,
            cancellationToken: Xunit.TestContext.Current.CancellationToken
        );
        user.EmailConfirmationCode.Should().NotBeNullOrEmpty();

        var confirmEmailModel = new ConfirmEmailModel
        {
            Code = user.EmailConfirmationCode,
            Email = email,
            DeviceFingerprint = "42"
        };
        var confirmEmailResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/confirm-email",
            confirmEmailModel
        );

        confirmEmailResult.Status.Should().Be(Status.Success);
        confirmEmailResult.Data.Token.Should().NotBeNullOrEmpty();
        confirmEmailResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        var token = confirmEmailResult.Data.Token;
        var refreshToken = confirmEmailResult.Data.RefreshToken;

        // valid
        var testResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/test",
            model: null,
            token
        );
        testResult.Status.Should().Be(Status.Success);

        // valid
        testResult = await PostAsync<OperationResult<bool>>("/api/auth/test", model: null, token);
        testResult.Status.Should().Be(Status.Success);

        // invalid
        await Task.Delay(
            int.Parse(_config["Jwt:AccessTokenExpirationInSeconds"]!) * 1000 + 10,
            Xunit.TestContext.Current.CancellationToken
        );
        testResult = await PostAsync<OperationResult<bool>>("/api/auth/test", model: null, token);
        testResult.Status.Should().Be(Status.AuthenticationFailed);

        // refresh
        var refreshTokenModel = new RefreshTokenModel { RefreshToken = refreshToken };
        var refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult.Status.Should().Be(Status.Success);
        refreshTokenResult.Data.Token.Should().NotBeNullOrEmpty();
        refreshTokenResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        token = refreshTokenResult.Data.Token;
        refreshToken = refreshTokenResult.Data.RefreshToken;

        // valid
        testResult = await PostAsync<OperationResult<bool>>("/api/auth/test", model: null, token);
        testResult.Status.Should().Be(Status.Success);

        // valid
        testResult = await PostAsync<OperationResult<bool>>("/api/auth/test", model: null, token);
        testResult.Status.Should().Be(Status.Success);

        // invalid
        await Task.Delay(
            int.Parse(_config["Jwt:RefreshTokenExpirationInSeconds"]!) * 1000 + 10,
            Xunit.TestContext.Current.CancellationToken
        );
        testResult = await PostAsync<OperationResult<bool>>("/api/auth/test", model: null, token);
        testResult.Status.Should().Be(Status.AuthenticationFailed);

        // refresh invalid
        refreshTokenModel = new RefreshTokenModel { RefreshToken = refreshToken };
        refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult.Status.Should().Be(Status.AuthenticationFailed);
        refreshTokenResult.Error.Should().Be("Срок действия рефреш токена истек");

        // проверка, что токен удалился
        refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult.Status.Should().Be(Status.AuthenticationFailed);
    }

    [Fact]
    public async Task no_remember_me_short_life_cycle()
    {
        // регистрация
        var email = $"{Guid.NewGuid()}@mail.ru";
        var registerModel = new RegisterModel
        {
            Email = email,
            Password = "42",
            ConfirmPassword = "42"
        };
        var registerResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/register",
            registerModel
        );

        registerResult.Status.Should().Be(Status.Success);
        registerResult.Data.Should().Be(true);

        // правильный код
        var user = await _db.Users.SingleAsync(
            u => u.Email == email,
            cancellationToken: Xunit.TestContext.Current.CancellationToken
        );
        user.EmailConfirmationCode.Should().NotBeNullOrEmpty();

        var confirmEmailModel = new ConfirmEmailModel
        {
            Code = user.EmailConfirmationCode,
            Email = email,
            DeviceFingerprint = "42"
        };
        var confirmEmailResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/confirm-email",
            confirmEmailModel
        );

        confirmEmailResult.Status.Should().Be(Status.Success);
        confirmEmailResult.Data.Token.Should().NotBeNullOrEmpty();
        confirmEmailResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        // login
        var loginModel = new LoginModel
        {
            Email = email,
            Password = "42",
            DeviceFingerprint = "42",
            RememberMe = false
        };
        var loginResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/login",
            loginModel
        );
        loginResult.Status.Should().Be(Status.Success);
        loginResult.Data.Token.Should().NotBeNullOrEmpty();
        loginResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        var token = loginResult.Data.Token;
        var refreshToken = loginResult.Data.RefreshToken;

        // valid
        var testResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/test",
            model: null,
            token
        );
        testResult.Status.Should().Be(Status.Success);

        // valid
        testResult = await PostAsync<OperationResult<bool>>("/api/auth/test", model: null, token);
        testResult.Status.Should().Be(Status.Success);

        // invalid
        await Task.Delay(
            int.Parse(_config["Jwt:AccessTokenExpirationInSeconds"]!) * 1000 + 10,
            Xunit.TestContext.Current.CancellationToken
        );
        testResult = await PostAsync<OperationResult<bool>>("/api/auth/test", model: null, token);
        testResult.Status.Should().Be(Status.AuthenticationFailed);

        // refresh
        var refreshTokenModel = new RefreshTokenModel { RefreshToken = refreshToken };
        var refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult.Status.Should().Be(Status.Success);
        refreshTokenResult.Data.Token.Should().NotBeNullOrEmpty();
        refreshTokenResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        token = refreshTokenResult.Data.Token;
        refreshToken = refreshTokenResult.Data.RefreshToken;

        // valid
        testResult = await PostAsync<OperationResult<bool>>("/api/auth/test", model: null, token);
        testResult.Status.Should().Be(Status.Success);

        // valid
        testResult = await PostAsync<OperationResult<bool>>("/api/auth/test", model: null, token);
        testResult.Status.Should().Be(Status.Success);

        // invalid
        await Task.Delay(
            int.Parse(_config["Jwt:ShortRefreshTokenExpirationInSeconds"]!) * 1000 + 10,
            Xunit.TestContext.Current.CancellationToken
        );
        testResult = await PostAsync<OperationResult<bool>>("/api/auth/test", model: null, token);
        testResult.Status.Should().Be(Status.AuthenticationFailed);

        // refresh invalid
        refreshTokenModel = new RefreshTokenModel { RefreshToken = refreshToken };
        refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult.Status.Should().Be(Status.AuthenticationFailed);
        refreshTokenResult.Error.Should().Be("Срок действия рефреш токена истек");

        // проверка, что токен удалился
        refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult.Status.Should().Be(Status.AuthenticationFailed);
    }

    [Fact]
    public async Task старый_рефреш_токен_валиден_в_течении_заданного_таймаута()
    {
        // регистрация
        var email = $"{Guid.NewGuid()}@mail.ru";
        var registerModel = new RegisterModel
        {
            Email = email,
            Password = "42",
            ConfirmPassword = "42"
        };
        var registerResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/register",
            registerModel
        );

        registerResult.Status.Should().Be(Status.Success);
        registerResult.Data.Should().Be(true);

        // правильный код
        var user = await _db.Users.SingleAsync(
            u => u.Email == email,
            cancellationToken: Xunit.TestContext.Current.CancellationToken
        );
        user.EmailConfirmationCode.Should().NotBeNullOrEmpty();

        var confirmEmailModel = new ConfirmEmailModel
        {
            Code = user.EmailConfirmationCode,
            Email = email,
            DeviceFingerprint = "42"
        };
        var confirmEmailResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/confirm-email",
            confirmEmailModel
        );

        confirmEmailResult.Status.Should().Be(Status.Success);
        confirmEmailResult.Data.Token.Should().NotBeNullOrEmpty();
        confirmEmailResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        // рефреш токенов
        var refreshTokenModel = new RefreshTokenModel
        {
            RefreshToken = confirmEmailResult.Data.RefreshToken
        };
        var refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult.Status.Should().Be(Status.Success);
        refreshTokenResult.Data.Token.Should().NotBeNullOrEmpty();
        refreshTokenResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        // аналогичный запрос, такой кейс возможен при быстром открытии вкладок
        var refreshTokenResult1 = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult1.Status.Should().Be(Status.Success);
        refreshTokenResult1.Data.Token.Should().NotBeNullOrEmpty();
        refreshTokenResult1.Data.RefreshToken.Should().NotBeNullOrEmpty();
        refreshTokenResult1.Data.RefreshToken.Should().Be(refreshTokenResult.Data.RefreshToken);

        await Task.Delay(
            int.Parse(_config["Jwt:RefreshTimeoutInSeconds"]!) * 1000 + 10,
            Xunit.TestContext.Current.CancellationToken
        );

        // а сейчас токен уже не валиден, т.к таймаут истек
        var refreshTokenResult2 = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult2.Status.Should().Be(Status.AuthenticationFailed);
    }

    [Fact]
    public async Task не_обновляем_токен_в_течении_заданного_таймаута()
    {
        // регистрация
        var email = $"{Guid.NewGuid()}@mail.ru";
        var registerModel = new RegisterModel
        {
            Email = email,
            Password = "42",
            ConfirmPassword = "42"
        };
        var registerResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/register",
            registerModel
        );

        registerResult.Status.Should().Be(Status.Success);
        registerResult.Data.Should().Be(true);

        // правильный код
        var user = await _db.Users.SingleAsync(
            u => u.Email == email,
            cancellationToken: Xunit.TestContext.Current.CancellationToken
        );
        user.EmailConfirmationCode.Should().NotBeNullOrEmpty();

        var confirmEmailModel = new ConfirmEmailModel
        {
            Code = user.EmailConfirmationCode,
            Email = email,
            DeviceFingerprint = "42"
        };
        var confirmEmailResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/confirm-email",
            confirmEmailModel
        );

        confirmEmailResult.Status.Should().Be(Status.Success);
        confirmEmailResult.Data.Token.Should().NotBeNullOrEmpty();
        confirmEmailResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        // рефреш токенов
        var refreshTokenModel = new RefreshTokenModel
        {
            RefreshToken = confirmEmailResult.Data.RefreshToken
        };
        var refreshTokenResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult.Status.Should().Be(Status.Success);
        refreshTokenResult.Data.Token.Should().NotBeNullOrEmpty();
        refreshTokenResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        // опять запрос на рефреш, но не обновляем - кейс возможен при быстром открытии вкладок
        refreshTokenModel.RefreshToken = refreshTokenResult.Data.RefreshToken;
        var refreshTokenResult1 = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult1.Status.Should().Be(Status.Success);
        refreshTokenResult1.Data.Token.Should().NotBeNullOrEmpty();
        refreshTokenResult1.Data.RefreshToken.Should().NotBeNullOrEmpty();
        refreshTokenResult1.Data.RefreshToken.Should().Be(refreshTokenResult.Data.RefreshToken);

        await Task.Delay(
            int.Parse(_config["Jwt:RefreshTimeoutInSeconds"]!) * 1000 + 10,
            Xunit.TestContext.Current.CancellationToken
        );

        // а сейчас обновляем, т.к таймаут истек
        var refreshTokenResult2 = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/refresh-tokens",
            refreshTokenModel
        );
        refreshTokenResult2.Status.Should().Be(Status.Success);
        refreshTokenResult2.Data.Token.Should().NotBeNullOrEmpty();
        refreshTokenResult2.Data.RefreshToken.Should().NotBeNullOrEmpty();
        refreshTokenResult2.Data.RefreshToken
            .Should()
            .NotBeEquivalentTo(refreshTokenResult1.Data.RefreshToken);
    }
}

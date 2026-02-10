using Devpull.Controllers;
using Devpull.Controllers.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationTests.Users;

public class LogoutTests : TestBase
{
    public LogoutTests(DatabaseFixture fixture)
        : base(fixture) { }

    [Fact]
    public async Task invalid_model_should_be_validation_errors()
    {
        var logoutModel = new LogoutModel { RefreshToken = string.Empty };
        var logoutResult = await PostAsync<OperationResult<bool>>("/api/auth/logout", logoutModel);

        logoutResult.Status.Should().Be(Status.ValidationFailed);
        logoutResult.ValidationErrors.Should().HaveCount(1);
        logoutResult.ValidationErrors
            .Should()
            .ContainKey("refreshToken")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Refresh Token обязателен");
    }

    [Fact]
    public async Task without_token_should_be_authentication_error()
    {
        var logoutModel = new LogoutModel { RefreshToken = "42" };
        var logoutResult = await PostAsync<OperationResult<bool>>("/api/auth/logout", logoutModel);

        logoutResult.Status.Should().Be(Status.AuthenticationFailed);
    }

    [Fact]
    public async Task invalid_token_should_be_authentication_error()
    {
        // выход
        var logoutModel = new LogoutModel { RefreshToken = "42" };
        var logoutResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/logout",
            logoutModel,
            token: "42"
        );

        logoutResult.Status.Should().Be(Status.AuthenticationFailed);
    }

    [Fact]
    public async Task valid_token_should_be_success()
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

        // выход
        var logoutModel = new LogoutModel { RefreshToken = confirmEmailResult.Data.RefreshToken };
        var logoutResult = await PostAsync<OperationResult<int>>(
            "/api/auth/logout",
            logoutModel,
            token: confirmEmailResult.Data.Token
        );

        logoutResult.Status.Should().Be(Status.Success);
        logoutResult.Data.Should().Be(1);
    }

    [Fact]
    public async Task valid_refresh_token_should_be_success()
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

        // ждем, пока токен протухнет
        await Task.Delay(
            int.Parse(_config["Jwt:AccessTokenExpirationInSeconds"]!) * 1000 + 10,
            Xunit.TestContext.Current.CancellationToken
        );

        // выход - ошибка
        var logoutModel = new LogoutModel { RefreshToken = "42" };
        var logoutResult = await PostAsync<OperationResult<int>>(
            "/api/auth/logout",
            logoutModel,
            token: confirmEmailResult.Data.Token
        );
        logoutResult.Status.Should().Be(Status.AuthenticationFailed);

        // refresh
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

        // выход
        logoutModel = new LogoutModel { RefreshToken = refreshTokenResult.Data.RefreshToken };
        logoutResult = await PostAsync<OperationResult<int>>(
            "/api/auth/logout",
            logoutModel,
            token: refreshTokenResult.Data.Token
        );

        logoutResult.Status.Should().Be(Status.Success);
        logoutResult.Data.Should().Be(1);
    }
}

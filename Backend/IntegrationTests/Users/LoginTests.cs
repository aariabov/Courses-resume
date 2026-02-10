using Devpull.Controllers;
using Devpull.Controllers.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationTests.Users;

public class LoginTests : TestBase
{
    public LoginTests(DatabaseFixture fixture)
        : base(fixture) { }

    [Fact]
    public async Task invalid_model_should_be_validation_errors()
    {
        // login
        var loginModel = new LoginModel();
        var loginResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/login",
            loginModel
        );

        loginResult.Status.Should().Be(Status.ValidationFailed);
        loginResult.ValidationErrors.Should().HaveCount(4);
        loginResult.ValidationErrors
            .Should()
            .ContainKey("email")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Email обязателен");
        loginResult.ValidationErrors
            .Should()
            .ContainKey("password")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Пароль обязателен");
        loginResult.ValidationErrors
            .Should()
            .ContainKey("deviceFingerprint")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Device Fingerprint обязателен");
        loginResult.ValidationErrors
            .Should()
            .ContainKey("rememberMe")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Флаг Запомнить меня обязательный");
    }

    [Fact]
    public async Task user_not_found_should_be_validation_errors()
    {
        // login
        var email = $"{Guid.NewGuid()}@mail.ru";
        var loginModel = new LoginModel
        {
            Email = email,
            Password = "42",
            DeviceFingerprint = "42",
            RememberMe = true
        };
        var loginResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/login",
            loginModel
        );

        loginResult.Status.Should().Be(Status.ValidationFailed);
        loginResult.ValidationErrors.Should().HaveCount(1);
        loginResult.ValidationErrors
            .Should()
            .ContainKey("email")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Пользователя с таким Email не существует");
    }

    [Fact]
    public async Task wrong_password_should_be_validation_errors()
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
            Password = "4242",
            DeviceFingerprint = "42",
            RememberMe = true
        };
        var loginResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/login",
            loginModel
        );
        loginResult.ValidationErrors.Should().HaveCount(1);
        loginResult.ValidationErrors
            .Should()
            .ContainKey("password")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Неправильный пароль");
    }

    [Fact]
    public async Task correct_password_should_be_success()
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
            RememberMe = true
        };
        var loginResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/login",
            loginModel
        );
        loginResult.Status.Should().Be(Status.Success);
        loginResult.Data.Token.Should().NotBeNullOrEmpty();
        loginResult.Data.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task два_логина_на_случай_если_фронт_очистит_токены()
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
            RememberMe = true
        };
        var loginResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/login",
            loginModel
        );
        loginResult.Status.Should().Be(Status.Success);
        loginResult.Data.Token.Should().NotBeNullOrEmpty();
        loginResult.Data.RefreshToken.Should().NotBeNullOrEmpty();

        // login
        loginModel = new LoginModel
        {
            Email = email,
            Password = "42",
            DeviceFingerprint = "42",
            RememberMe = true
        };
        loginResult = await PostAsync<OperationResult<TokensModel>>("/api/auth/login", loginModel);
        loginResult.Status.Should().Be(Status.Success);
        loginResult.Data.Token.Should().NotBeNullOrEmpty();
        loginResult.Data.RefreshToken.Should().NotBeNullOrEmpty();
    }
}

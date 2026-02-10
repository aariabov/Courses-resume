using System.Net.Http.Headers;
using System.Text;
using Devpull.Controllers;
using Devpull.Controllers.Models;
using Devpull.DbModels;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests.Users;

public class RegistrationTests : TestBase
{
    public RegistrationTests(DatabaseFixture fixture)
        : base(fixture) { }

    [Fact]
    public async Task invalid_model_should_be_validation_errors()
    {
        // регистрация
        var registerModel = new RegisterModel();
        var registerResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/register",
            registerModel
        );

        registerResult.Status.Should().Be(Status.ValidationFailed);
        registerResult.ValidationErrors.Should().HaveCount(3);
        registerResult.ValidationErrors
            .Should()
            .ContainKey("email")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Email обязателен");
        registerResult.ValidationErrors
            .Should()
            .ContainKey("password")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Пароль обязателен");
        registerResult.ValidationErrors
            .Should()
            .ContainKey("confirmPassword")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Подтвердите пароль");
    }

    [Fact]
    public async Task invalid_confirm_email_model_should_be_validation_errors()
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

        // невалидная модель
        var confirmEmailModel = new ConfirmEmailModel { Code = "123" };
        var confirmEmailResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/confirm-email",
            confirmEmailModel
        );

        confirmEmailResult.Status.Should().Be(Status.ValidationFailed);
        confirmEmailResult.ValidationErrors.Should().HaveCount(3);
        confirmEmailResult.ValidationErrors
            .Should()
            .ContainKey("email")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Email обязателен");
        confirmEmailResult.ValidationErrors
            .Should()
            .ContainKey("code")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Введите 6-ти значный код");
        confirmEmailResult.ValidationErrors
            .Should()
            .ContainKey("deviceFingerprint")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Device Fingerprint обязателен");
    }

    [Fact]
    public async Task invalid_code_should_be_validation_errors()
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

        // неправильный код
        var confirmEmailModel = new ConfirmEmailModel
        {
            Code = "------",
            Email = email,
            DeviceFingerprint = "42"
        };
        var confirmEmailResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/confirm-email",
            confirmEmailModel
        );

        confirmEmailResult.Status.Should().Be(Status.ValidationFailed);
        confirmEmailResult.ValidationErrors
            .Should()
            .ContainKey("code")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Неправильный код");
    }

    [Fact]
    public async Task expired_code_should_be_validation_errors()
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

        // просроченный код
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
        await Task.Delay(
            int.Parse(_config["User:ValidCodeTimeInSeconds"]!) * 1000 + 10,
            Xunit.TestContext.Current.CancellationToken
        );
        var confirmEmailResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/confirm-email",
            confirmEmailModel
        );

        confirmEmailResult.Status.Should().Be(Status.ValidationFailed);
        confirmEmailResult.ValidationErrors
            .Should()
            .ContainKey("code")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Истек срок действия кода");
    }

    [Fact]
    public async Task valid_code_should_be_success()
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
    }

    [Fact]
    public async Task not_confirm_user_in_db_valid_code_should_be_success()
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

        // повторная регистрация (например, если юзер в первый раз не подтвердил email)
        registerResult = await PostAsync<OperationResult<bool>>(
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
    }

    [Fact]
    public async Task user_exists_already_should_be_validation_errors()
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

        // пользователь пытается повторно зарегистрироваться
        registerModel = new RegisterModel
        {
            Email = email,
            Password = "4242",
            ConfirmPassword = "4242"
        };
        registerResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/register",
            registerModel
        );

        registerResult.Status.Should().Be(Status.ValidationFailed);
        registerResult.ValidationErrors
            .Should()
            .ContainKey("email")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Пользователь уже существует, попробуйте Войти");
    }
}

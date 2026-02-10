using Devpull.Controllers;
using Devpull.Controllers.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IntegrationTests.Users;

public class ResetPasswordTests : TestBase
{
    public ResetPasswordTests(DatabaseFixture fixture)
        : base(fixture) { }

    [Fact]
    public async Task invalid_forgot_passport_model_should_be_validation_errors()
    {
        var model = new ForgotPasswordModel();
        var result = await PostAsync<OperationResult<bool>>("/api/auth/forgot-password", model);

        result.Status.Should().Be(Status.ValidationFailed);
        result.ValidationErrors.Should().HaveCount(1);
        result.ValidationErrors
            .Should()
            .ContainKey("email")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Email обязателен");
    }

    [Fact]
    public async Task not_found_user_should_be_validation_errors()
    {
        var email = $"{Guid.NewGuid()}@mail.ru";
        var model = new ForgotPasswordModel { Email = email };
        var result = await PostAsync<OperationResult<bool>>("/api/auth/forgot-password", model);

        result.Status.Should().Be(Status.ValidationFailed);
        result.ValidationErrors.Should().HaveCount(1);
        result.ValidationErrors
            .Should()
            .ContainKey("email")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Пользователя с таким Email не существует");
    }

    [Fact]
    public async Task invalid_reset_passport_model_should_be_validation_errors()
    {
        var model = new ResetPasswordModel();
        var result = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/reset-password",
            model
        );

        result.Status.Should().Be(Status.ValidationFailed);
        result.ValidationErrors.Should().HaveCount(5);
        result.ValidationErrors
            .Should()
            .ContainKey("email")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Email обязателен");
        result.ValidationErrors
            .Should()
            .ContainKey("code")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Код обязателен");
        result.ValidationErrors
            .Should()
            .ContainKey("deviceFingerprint")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Device Fingerprint обязателен");
        result.ValidationErrors
            .Should()
            .ContainKey("password")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Пароль обязателен");
        result.ValidationErrors
            .Should()
            .ContainKey("confirmPassword")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Подтвердите пароль");
    }

    [Fact(
        DisplayName = "юзер не подтвердил email при регистрации, а потом пытается восстановить пароль"
    )]
    public async Task not_confirm_user_should_be_validation_errors()
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

        // запрос на восстановление пароля
        var forgotPasswordModel = new ForgotPasswordModel { Email = email };
        var forgotPassportResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/forgot-password",
            forgotPasswordModel
        );
        forgotPassportResult.Status.Should().Be(Status.ValidationFailed);
        forgotPassportResult.ValidationErrors.Should().HaveCount(1);
        forgotPassportResult.ValidationErrors
            .Should()
            .ContainKey("email")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Пользователя с таким Email не существует");
    }

    // юзер сразу дергает reset
    [Fact]
    public async Task user_call_reset_passport_without_forgot_should_be_validation_errors()
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

        // сброс пароля
        var resetPasswordModel = new ResetPasswordModel
        {
            Email = email,
            Code = "------",
            Password = "42",
            ConfirmPassword = "42",
            DeviceFingerprint = "42"
        };
        var resetPasswordResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/reset-password",
            resetPasswordModel
        );
        resetPasswordResult.Status.Should().Be(Status.ValidationFailed);
        resetPasswordResult.ValidationErrors
            .Should()
            .ContainKey("code")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Истек срок действия кода");
    }

    // неправильный код
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

        // запрос на восстановление пароля
        var forgotPasswordModel = new ForgotPasswordModel { Email = email };
        var forgotPassportResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/forgot-password",
            forgotPasswordModel
        );
        forgotPassportResult.Status.Should().Be(Status.Success);

        // сброс пароля, неправильный код
        var resetPasswordModel = new ResetPasswordModel
        {
            Email = email,
            Code = "------",
            Password = "42",
            ConfirmPassword = "42",
            DeviceFingerprint = "42"
        };
        var resetPasswordResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/reset-password",
            resetPasswordModel
        );
        resetPasswordResult.Status.Should().Be(Status.ValidationFailed);
        resetPasswordResult.ValidationErrors
            .Should()
            .ContainKey("code")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Неправильный код");
    }

    // просроченный код
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

        // запрос на восстановление пароля
        var forgotPasswordModel = new ForgotPasswordModel { Email = email };
        var forgotPassportResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/forgot-password",
            forgotPasswordModel
        );
        forgotPassportResult.Status.Should().Be(Status.Success);

        // сброс пароля, просроченный код
        await Task.Delay(
            int.Parse(_config["User:ValidCodeTimeInSeconds"]!) * 1000 + 10,
            Xunit.TestContext.Current.CancellationToken
        );
        var resetPasswordModel = new ResetPasswordModel
        {
            Email = email,
            Code = "------",
            Password = "42",
            ConfirmPassword = "42",
            DeviceFingerprint = "42"
        };
        var resetPasswordResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/reset-password",
            resetPasswordModel
        );
        resetPasswordResult.Status.Should().Be(Status.ValidationFailed);
        resetPasswordResult.ValidationErrors
            .Should()
            .ContainKey("code")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Истек срок действия кода");
    }

    // успех
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

        // запрос на восстановление пароля
        var forgotPasswordModel = new ForgotPasswordModel { Email = email };
        var forgotPassportResult = await PostAsync<OperationResult<bool>>(
            "/api/auth/forgot-password",
            forgotPasswordModel
        );
        forgotPassportResult.Status.Should().Be(Status.Success);

        // сброс пароля, правильный код
        user = await _db.Users.SingleAsync(
            u => u.Email == email,
            cancellationToken: Xunit.TestContext.Current.CancellationToken
        );
        user.EmailConfirmationCode.Should().NotBeNullOrEmpty();
        var resetPasswordModel = new ResetPasswordModel
        {
            Email = email,
            Code = user.EmailConfirmationCode,
            Password = "42",
            ConfirmPassword = "42",
            DeviceFingerprint = "42"
        };
        var resetPasswordResult = await PostAsync<OperationResult<TokensModel>>(
            "/api/auth/reset-password",
            resetPasswordModel
        );
        resetPasswordResult.Status.Should().Be(Status.Success);
        resetPasswordResult.Data.Token.Should().NotBeNullOrEmpty();
        resetPasswordResult.Data.RefreshToken.Should().NotBeNullOrEmpty();
    }
}

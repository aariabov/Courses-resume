using Devpull.Controllers;
using Devpull.Controllers.Models;
using Devpull.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace UnitTests.Users;

[TestClass]
public class RegisterModelValidatorTests
{
    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    public async Task EmailIsEmpty_ErrorMessage(string? email)
    {
        var model = new RegisterModel
        {
            Email = email!,
            Password = "42",
            ConfirmPassword = "42"
        };
        var userManagerMock = new Mock<IUserManagerService>();

        var validator = new RegisterModelValidator(userManagerMock.Object);

        var ex = await Assert.ThrowsExceptionAsync<ValidationFailException>(
            () => validator.Validate(model)
        );

        Assert.IsTrue(ex.ErrorsList.TryGetValue("email", out var errors));
        Assert.AreEqual(1, errors.Count);
        CollectionAssert.Contains(errors, "Email обязателен");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    public async Task PasswordIsEmpty_ErrorMessage(string? password)
    {
        var model = new RegisterModel
        {
            Email = "42@mail.ru",
            Password = password!,
            ConfirmPassword = "42"
        };
        var userManagerMock = new Mock<IUserManagerService>();

        var validator = new RegisterModelValidator(userManagerMock.Object);

        var ex = await Assert.ThrowsExceptionAsync<ValidationFailException>(
            () => validator.Validate(model)
        );

        ex.ErrorsList
            .Should()
            .ContainKey("password")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Пароль обязателен");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    public async Task ConfirmPasswordIsEmpty_ErrorMessage(string? confirmPassword)
    {
        var model = new RegisterModel
        {
            Email = "42@mail.ru",
            Password = "42",
            ConfirmPassword = confirmPassword!
        };
        var userManagerMock = new Mock<IUserManagerService>();

        var validator = new RegisterModelValidator(userManagerMock.Object);

        var ex = await Assert.ThrowsExceptionAsync<ValidationFailException>(
            () => validator.Validate(model)
        );

        ex.ErrorsList
            .Should()
            .ContainKey("confirmPassword")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Подтвердите пароль");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    public async Task AllEmpty_ErrorMessages(string? str)
    {
        var model = new RegisterModel
        {
            Email = str!,
            Password = str!,
            ConfirmPassword = str!
        };
        var userManagerMock = new Mock<IUserManagerService>();

        var validator = new RegisterModelValidator(userManagerMock.Object);

        var ex = await Assert.ThrowsExceptionAsync<ValidationFailException>(
            () => validator.Validate(model)
        );

        ex.ErrorsList.Should().HaveCount(3);
        ex.ErrorsList
            .Should()
            .ContainKey("email")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Email обязателен");
        ex.ErrorsList
            .Should()
            .ContainKey("password")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Пароль обязателен");
        ex.ErrorsList
            .Should()
            .ContainKey("confirmPassword")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Подтвердите пароль");
    }

    [TestMethod]
    public async Task PasswordsAreNotEqual_ErrorMessage()
    {
        var model = new RegisterModel
        {
            Email = "42@mail.ru",
            Password = "42",
            ConfirmPassword = "4242"
        };
        var userManagerMock = new Mock<IUserManagerService>();

        var validator = new RegisterModelValidator(userManagerMock.Object);

        var ex = await Assert.ThrowsExceptionAsync<ValidationFailException>(
            () => validator.Validate(model)
        );

        ex.ErrorsList
            .Should()
            .ContainKey("confirmPassword")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Пароли не совпадают");
    }

    [TestMethod]
    public async Task UserExistsAlready_ErrorMessage()
    {
        var existingUser = new AppUser { Email = "42@mail.ru", EmailConfirmed = true };
        var model = new RegisterModel
        {
            Email = existingUser.Email,
            Password = "42",
            ConfirmPassword = "42"
        };
        var userManagerMock = new Mock<IUserManagerService>();
        userManagerMock
            .Setup(m => m.FindByEmailMaybeAsync(existingUser.Email))
            .ReturnsAsync(existingUser);

        var validator = new RegisterModelValidator(userManagerMock.Object);

        var ex = await Assert.ThrowsExceptionAsync<ValidationFailException>(
            () => validator.Validate(model)
        );

        ex.ErrorsList
            .Should()
            .ContainKey("email")
            .WhoseValue.Should()
            .HaveCount(1)
            .And.Contain("Пользователь уже существует, попробуйте Войти");
    }

    [TestMethod]
    public async Task UserNotExists_NoErrors()
    {
        var model = new RegisterModel
        {
            Email = "42@mail.ru",
            Password = "42",
            ConfirmPassword = "42"
        };
        var userManagerMock = new Mock<IUserManagerService>();
        userManagerMock
            .Setup(m => m.FindByEmailMaybeAsync(model.Email))
            .ReturnsAsync((AppUser?)null);

        var validator = new RegisterModelValidator(userManagerMock.Object);

        await validator.Validate(model);
    }

    [DataTestMethod]
    [DataRow("qwe")]
    [DataRow("123324")]
    [DataRow("test.ru")]
    [DataRow("test.com")]
    [DataRow("test@ru")]
    [DataRow("test@com")]
    [DataRow("test@mail")]
    [DataRow("test@mail.")]
    [DataRow("test@mail")]
    [DataRow("test@mail.r")]
    [DataRow("test@mail")]
    [DataRow("test@mail.r")]
    [DataRow("eхаmplе@rambler.com")] // содержит русские буквы
    [DataRow("primer2mail.ru")]
    [DataRow("primer3@mailru")]
    [DataRow("primer5—@mail.ru")]
    [DataRow("primer&[email protected]")]
    [DataRow("userexample.com")]
    [DataRow("userdomain.com")]
    [DataRow("user name@example.com")]
    [DataRow("user@domain .com")]
    [DataRow("user@name@example.com")]
    [DataRow("user@.com")]
    [DataRow("@example.com")]
    [DataRow("@domain.com")]
    [DataRow("user@")]
    [DataRow("user@.")]
    [DataRow("user@example.c")]
    [DataRow("user@domain.123")]
    [DataRow("a@b.c")]
    //[DataRow("ser@domain.thisdomainnameistoolongtobevalid")]
    [DataRow("user@exa mple.com")]
    [DataRow("user@exa[mple.com")]
    [DataRow("plainaddress")]
    [DataRow("@example.com")]
    [DataRow("user@.com")]
    [DataRow("user@example.com.")]
    //[DataRow("user@-example.com")]
    [DataRow("user@example_com")]
    [DataRow("user@exam ple.com")]
    [DataRow("user@ex!ample.com")]
    //[DataRow("user@.example.com")]
    //[DataRow("user@example..com")]
    [DataRow("user@example.c")]
    [DataRow("us er@example.com")]
    [DataRow("user@localhost")]
    public async Task EmailIsIncorrect_ErrorMessage(string email)
    {
        var model = new RegisterModel { Email = email! };
        var userManagerMock = new Mock<IUserManagerService>();

        var validator = new RegisterModelValidator(userManagerMock.Object);

        var ex = await Assert.ThrowsExceptionAsync<ValidationFailException>(
            () => validator.Validate(model)
        );

        Assert.IsTrue(ex.ErrorsList.TryGetValue("email", out var errors));
        CollectionAssert.Contains(errors, "Некорректный формат Email");
    }
}

using AutoFixture;
using AutoFixture.AutoMoq;
using Devpull.Common;
using Devpull.Controllers.Models;
using Devpull.DbModels;
using Devpull.Users;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace UnitTests.Users;

[TestClass]
public class RefreshTokensTests
{
    [TestMethod]
    public async Task RefreshTokens_Should_Handle_Race_Condition_Correctly()
    {
        // Arrange
        var oldRefreshToken = "refresh-token-123";
        var userId = "user1";
        var refreshToken = new RefreshToken
        {
            Token = oldRefreshToken,
            UserId = userId,
            ExpiryDate = DateTime.UtcNow.AddMinutes(5),
            DeviceFingerprint = "4242",
            OldToken = null,
            RememberMe = true,
            RefreshDate = DateTime.UtcNow.AddSeconds(-15),
        };

        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        var appConfig = new AppConfig { Jwt = { RefreshTimeoutInSeconds = 10 } };
        fixture.Inject(appConfig);

        var mockTokenService = fixture.Freeze<Mock<ITokenService>>();
        mockTokenService
            .Setup(u => u.GetRefreshToken(oldRefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        var userService = fixture.Create<UserService>();

        var model = new RefreshTokenModel { RefreshToken = oldRefreshToken };
        var cts = CancellationToken.None;

        // Act: Запускаем два параллельных запроса refresh одновременно
        var task1 = userService.RefreshTokens(model, cts);
        var task2 = userService.RefreshTokens(model, cts);

        var results = await Task.WhenAll(task1, task2);

        // Assert
        Assert.IsNotNull(results[0]);
        Assert.IsNotNull(results[1]);
        Assert.AreEqual(results[0].RefreshToken, results[1].RefreshToken); // Один и тот же refreshToken
        mockTokenService.Verify(
            x => x.UpdateRefreshToken(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Once // Обновление refresh токена должно быть только 1 раз
        );
    }

    [TestMethod]
    public async Task симуляция_гонки_при_2х_запросах_со_старыми_решреш_токенами()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        var appConfig = new AppConfig { Jwt = { RefreshTimeoutInSeconds = 2 } };
        fixture.Inject(appConfig);

        const string oldRefreshToken = "oldRefreshToken";
        const string newRefreshToken = "newRefreshToken";

        // первая запись вернется при первом рефреше, а вторая при втором запросе
        var tokens = new Queue<RefreshToken>(
            new[]
            {
                new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = oldRefreshToken,
                    DeviceFingerprint = "4242",
                    ExpiryDate = DateTime.UtcNow.AddMinutes(5),
                    UserId = "42",
                    OldToken = null,
                    RefreshDate = DateTime.UtcNow.AddSeconds(-15),
                    RememberMe = true
                },
                new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = newRefreshToken,
                    DeviceFingerprint = "4242",
                    ExpiryDate = DateTime.UtcNow.AddMinutes(5),
                    UserId = "42",
                    OldToken = oldRefreshToken,
                    RefreshDate = DateTime.UtcNow,
                    RememberMe = true
                }
            }
        );

        var mockTokenService = fixture.Freeze<Mock<ITokenService>>();
        mockTokenService
            .Setup(u => u.GetRefreshToken(oldRefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => tokens.Dequeue());

        mockTokenService.Setup(u => u.GenerateRefreshToken()).Returns(newRefreshToken);

        // Симуляция гонки: первый запрос "зависает" на UpdateRefreshToken, пока второй выполняется
        var updateCalled = new TaskCompletionSource();
        var continueUpdate = new TaskCompletionSource();

        mockTokenService
            .Setup(
                s => s.UpdateRefreshToken(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>())
            )
            .Returns(async () =>
            {
                if (!updateCalled.Task.IsCompleted)
                {
                    updateCalled.TrySetResult();
                }

                await continueUpdate.Task; // блокируем, пока не дадим разрешение из теста
                return 1;
            });

        var userService = fixture.Create<UserService>();

        var refreshTokenModel = new RefreshTokenModel { RefreshToken = oldRefreshToken };
        // запускаем 1ый запрос
        var task1 = userService.RefreshTokens(refreshTokenModel, CancellationToken.None);
        // ждём, пока первый запрос дойдёт до UpdateRefreshToken
        await updateCalled.Task;

        // запускаем 2ой запрос
        var task2 = userService.RefreshTokens(refreshTokenModel, CancellationToken.None);

        // разрешаем первому запросу закончить обновление
        continueUpdate.TrySetResult();

        var result1 = await task1;
        var result2 = await task2;

        mockTokenService.Verify(
            m => m.UpdateRefreshToken(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        mockTokenService.Verify(m => m.GenerateRefreshToken(), Times.Once);
        Console.WriteLine(result1.RefreshToken);
        Console.WriteLine(result2.RefreshToken);
        result2.RefreshToken.Should().Be(result1.RefreshToken);
    }
}

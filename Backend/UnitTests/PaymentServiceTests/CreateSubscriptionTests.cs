using Devpull.Controllers;
using Devpull.DbModels;
using Devpull.Payments;
using FluentAssertions;
using Moq;

namespace UnitTests.PaymentServiceTests;

[TestClass]
public class CreateSubscriptionTests
{
    [TestMethod]
    public void нет_подписок_создаем_на_месяц_start_date_now()
    {
        const string userId = "123";
        const string paymentId = "42";
        var appUser = new AppUser { Id = userId, Email = "test@test.com", };

        appUser.CreateSubscription(paymentId, PaymentType.PerMonth);

        appUser.Subscriptions.Count.Should().Be(1);
        var subscription = appUser.Subscriptions.Single();
        subscription.StartDate.Date.Should().Be(DateTime.UtcNow.Date);
        subscription.EndDate.Date.Should().Be(DateTime.UtcNow.Date.AddDays(31));
        subscription.PaymentId.Should().Be(paymentId);
        subscription.UserId.Should().Be(userId);
    }

    [TestMethod]
    public void нет_подписок_создаем_на_год_start_date_now()
    {
        const string userId = "123";
        const string paymentId = "42";
        var appUser = new AppUser { Id = userId, Email = "test@test.com", };

        appUser.CreateSubscription(paymentId, PaymentType.PerYear);

        appUser.Subscriptions.Count.Should().Be(1);
        var subscription = appUser.Subscriptions.Single();
        subscription.StartDate.Date.Should().Be(DateTime.UtcNow.Date);
        subscription.EndDate.Date.Should().Be(DateTime.UtcNow.Date.AddYears(1));
        subscription.PaymentId.Should().Be(paymentId);
        subscription.UserId.Should().Be(userId);
    }

    [TestMethod]
    public void есть_просроченная_подписка_создаем_start_date_now()
    {
        const string userId = "123";
        const string paymentId = "42";
        var appUser = new AppUser
        {
            Id = userId,
            Email = "test@test.com",
            Subscriptions = new List<Subscription>
            {
                new()
                {
                    StartDate = DateTime.UtcNow.AddMonths(-1),
                    EndDate = DateTime.UtcNow.AddSeconds(-1),
                    UserId = userId,
                    PaymentId = "any",
                }
            }
        };

        appUser.CreateSubscription(paymentId, PaymentType.PerMonth);

        appUser.Subscriptions.Count.Should().Be(2);
        var subscription = appUser.Subscriptions.OrderBy(s => s.EndDate).Last();
        subscription.StartDate.Date.Should().Be(DateTime.UtcNow.Date);
        subscription.EndDate.Date.Should().Be(DateTime.UtcNow.Date.AddDays(31));
        subscription.PaymentId.Should().Be(paymentId);
        subscription.UserId.Should().Be(userId);
    }

    [TestMethod]
    public void есть_активная_подписка_создаем_start_date_это_end_date_активной_подписки()
    {
        const string userId = "123";
        const string paymentId = "42";
        var endDate = DateTime.UtcNow.AddDays(1);
        var appUser = new AppUser
        {
            Id = userId,
            Email = "test@test.com",
            Subscriptions = new List<Subscription>
            {
                new()
                {
                    StartDate = DateTime.UtcNow.AddMonths(-1),
                    EndDate = endDate,
                    UserId = userId,
                    PaymentId = "any",
                }
            }
        };

        appUser.CreateSubscription(paymentId, PaymentType.PerMonth);

        appUser.Subscriptions.Count.Should().Be(2);
        var subscription = appUser.Subscriptions.OrderBy(s => s.EndDate).Last();
        subscription.StartDate.Should().Be(endDate);
        subscription.EndDate.Should().Be(endDate.AddDays(31));
        subscription.PaymentId.Should().Be(paymentId);
        subscription.UserId.Should().Be(userId);
    }

    [TestMethod]
    public void есть_активная_и_будущая_подписка_создаем_start_date_это_end_date_будущей_подписки()
    {
        const string userId = "123";
        const string paymentId = "42";
        var activeEndDate = DateTime.UtcNow.AddDays(1);
        var futureEndDate = activeEndDate.AddMonths(1);
        var appUser = new AppUser
        {
            Id = userId,
            Email = "test@test.com",
            Subscriptions = new List<Subscription>
            {
                new()
                {
                    StartDate = DateTime.UtcNow.AddMonths(-1),
                    EndDate = activeEndDate,
                    UserId = userId,
                    PaymentId = "any",
                },
                new()
                {
                    StartDate = activeEndDate,
                    EndDate = futureEndDate,
                    UserId = userId,
                    PaymentId = "any",
                }
            }
        };

        appUser.CreateSubscription(paymentId, PaymentType.PerMonth);

        appUser.Subscriptions.Count.Should().Be(3);
        var subscription = appUser.Subscriptions.OrderBy(s => s.EndDate).Last();
        subscription.StartDate.Should().Be(futureEndDate);
        subscription.EndDate.Should().Be(futureEndDate.AddDays(31));
        subscription.PaymentId.Should().Be(paymentId);
        subscription.UserId.Should().Be(userId);
    }

    [TestMethod]
    public void есть_активная_и_две_будущих_подписки_создаем_start_date_это_end_date_будущей_подписки()
    {
        const string userId = "123";
        const string paymentId = "42";
        var activeEndDate = DateTime.UtcNow.AddDays(1);
        var futureEndDate = activeEndDate.AddMonths(1);
        var lastFutureEndDate = activeEndDate.AddYears(1);
        var appUser = new AppUser
        {
            Id = userId,
            Email = "test@test.com",
            Subscriptions = new List<Subscription>
            {
                new()
                {
                    StartDate = DateTime.UtcNow.AddMonths(-1),
                    EndDate = activeEndDate,
                    UserId = userId,
                    PaymentId = "any",
                },
                new()
                {
                    StartDate = activeEndDate,
                    EndDate = futureEndDate,
                    UserId = userId,
                    PaymentId = "any",
                },
                new()
                {
                    StartDate = futureEndDate,
                    EndDate = lastFutureEndDate,
                    UserId = userId,
                    PaymentId = "any",
                }
            }
        };

        appUser.CreateSubscription(paymentId, PaymentType.PerYear);

        appUser.Subscriptions.Count.Should().Be(4);
        var subscription = appUser.Subscriptions.OrderBy(s => s.EndDate).Last();
        subscription.StartDate.Should().Be(lastFutureEndDate);
        subscription.EndDate.Should().Be(lastFutureEndDate.AddYears(1));
        subscription.PaymentId.Should().Be(paymentId);
        subscription.UserId.Should().Be(userId);
    }
}

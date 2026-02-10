using System.Text.Json;
using AutoFixture;
using Devpull.Controllers;
using Devpull.DbModels;

namespace UnitTests.Users;

[TestClass]
public class CanUserRunExerciseTests
{
    [TestMethod]
    public void нет_подписки_нет_запусков()
    {
        var user = new AppUser { Email = string.Empty, };
        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsTrue(canRun);
        Assert.IsNull(fromDate);
    }

    [TestMethod]
    public void нет_подписки_есть_1_запуск()
    {
        var dates = new[] { DateTime.UtcNow.AddMinutes(-10) };
        var runs = CreateRunExercises(dates);

        var user = new AppUser { Email = string.Empty, RunExercises = runs };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsTrue(canRun);
        Assert.IsNull(fromDate);
    }

    [TestMethod]
    public void нет_подписки_есть_2_запуска()
    {
        var dates = new[] { DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(-5) };
        var runs = CreateRunExercises(dates);

        var user = new AppUser { Email = string.Empty, RunExercises = runs };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsTrue(canRun);
        Assert.IsNull(fromDate);
    }

    [TestMethod]
    public void нет_подписки_есть_3_запуска_запрещено()
    {
        var firstRunDate = DateTime.UtcNow.AddMinutes(-10);
        var expectedFromDate = firstRunDate.AddDays(1);
        var dates = new[]
        {
            firstRunDate,
            DateTime.UtcNow.AddMinutes(-5),
            DateTime.UtcNow.AddMinutes(-3)
        };
        var runs = CreateRunExercises(dates);

        var user = new AppUser { Email = string.Empty, RunExercises = runs };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsFalse(canRun);
        Assert.AreEqual(expectedFromDate, fromDate);
    }

    [TestMethod]
    public void нет_подписки_есть_3_запуска_запрещено_близко()
    {
        var firstRunDate = DateTime.UtcNow.AddHours(-23).AddMinutes(-59);
        var expectedFromDate = firstRunDate.AddDays(1);
        var dates = new[]
        {
            firstRunDate,
            DateTime.UtcNow.AddMinutes(-5),
            DateTime.UtcNow.AddMinutes(-3)
        };
        var runs = CreateRunExercises(dates);

        var user = new AppUser { Email = string.Empty, RunExercises = runs };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsFalse(canRun);
        Assert.AreEqual(expectedFromDate, fromDate);
    }

    [TestMethod]
    public void нет_подписки_есть_3_запуска_но_уже_след_день()
    {
        var firstRunDate = DateTime.UtcNow.AddHours(-24);
        var dates = new[]
        {
            firstRunDate,
            DateTime.UtcNow.AddMinutes(-5),
            DateTime.UtcNow.AddMinutes(-3)
        };
        var runs = CreateRunExercises(dates);

        var user = new AppUser { Email = string.Empty, RunExercises = runs };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsTrue(canRun);
    }

    [TestMethod]
    public void нет_подписки_есть_4_запуска_где_3_были_вчера()
    {
        var firstRunDate = DateTime.UtcNow.AddHours(-24).AddMinutes(-1);
        var dates = new[]
        {
            firstRunDate,
            firstRunDate.AddSeconds(3),
            firstRunDate.AddMinutes(5),
            DateTime.UtcNow.AddMinutes(-3)
        };
        var runs = CreateRunExercises(dates);

        var user = new AppUser { Email = string.Empty, RunExercises = runs };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsTrue(canRun);
    }

    [TestMethod]
    public void нет_подписки_есть_5_запусков_где_3_были_вчера()
    {
        var firstRunDate = DateTime.UtcNow.AddHours(-24).AddMinutes(-6);
        var dates = new[]
        {
            firstRunDate,
            firstRunDate.AddSeconds(3),
            firstRunDate.AddMinutes(5),
            DateTime.UtcNow.AddMinutes(-5),
            DateTime.UtcNow.AddMinutes(-3)
        };
        var runs = CreateRunExercises(dates);

        var user = new AppUser { Email = string.Empty, RunExercises = runs };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsTrue(canRun);
    }

    [TestMethod]
    public void нет_подписки_есть_6_запусков_запрещено()
    {
        var lastRunDate = DateTime.UtcNow.AddHours(-24).AddMinutes(-6);
        var firstRunDate = DateTime.UtcNow.AddMinutes(-5);
        var expectedFromDate = firstRunDate.AddDays(1);
        var dates = new[]
        {
            lastRunDate,
            lastRunDate.AddSeconds(3),
            lastRunDate.AddMinutes(5),
            firstRunDate,
            DateTime.UtcNow.AddMinutes(-3),
            DateTime.UtcNow.AddSeconds(-20)
        };
        var runs = CreateRunExercises(dates);

        var user = new AppUser { Email = string.Empty, RunExercises = runs };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsFalse(canRun);
        Assert.AreEqual(expectedFromDate, fromDate);
    }

    [TestMethod]
    public void есть_актуальная_подписка()
    {
        var dates = new (DateTime StartDate, DateTime EndDate)[]
        {
            new(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1)),
        };
        var subscriptions = CreateSubscriptions(dates);

        var user = new AppUser { Email = string.Empty, Subscriptions = subscriptions };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsTrue(canRun);
        Assert.IsNull(fromDate);
    }

    [TestMethod]
    public void есть_прошедшая_подписка_и_нет_3х_запусков()
    {
        var dates = new (DateTime StartDate, DateTime EndDate)[]
        {
            new(DateTime.UtcNow.AddMinutes(-3), DateTime.UtcNow.AddMinutes(-1)),
        };
        var subscriptions = CreateSubscriptions(dates);

        var user = new AppUser { Email = string.Empty, Subscriptions = subscriptions };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsTrue(canRun);
        Assert.IsNull(fromDate);
    }

    [TestMethod]
    public void есть_прошедшая_подписка_и_есть_3_запуска_запрещено()
    {
        var dates = new (DateTime StartDate, DateTime EndDate)[]
        {
            new(DateTime.UtcNow.AddMinutes(-3), DateTime.UtcNow.AddMinutes(-1)),
        };
        var subscriptions = CreateSubscriptions(dates);
        var firstRunDate = DateTime.UtcNow.AddMinutes(-10);
        var expectedFromDate = firstRunDate.AddDays(1);
        var runDates = new[]
        {
            firstRunDate,
            DateTime.UtcNow.AddMinutes(-5),
            DateTime.UtcNow.AddMinutes(-3)
        };
        var runs = CreateRunExercises(runDates);

        var user = new AppUser
        {
            Email = string.Empty,
            Subscriptions = subscriptions,
            RunExercises = runs
        };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsFalse(canRun);
        Assert.AreEqual(expectedFromDate, fromDate);
    }

    [TestMethod]
    public void есть_прошедшая_и_актуальная_подписка()
    {
        var dates = new (DateTime StartDate, DateTime EndDate)[]
        {
            new(DateTime.UtcNow.AddMinutes(-3), DateTime.UtcNow.AddMinutes(-1)),
            new(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1)),
        };
        var subscriptions = CreateSubscriptions(dates);

        var user = new AppUser { Email = string.Empty, Subscriptions = subscriptions };

        var (canRun, fromDate) = user.CanUserRunExercise(3);

        Assert.IsTrue(canRun);
        Assert.IsNull(fromDate);
    }

    private static Subscription[] CreateSubscriptions(
        (DateTime StartDate, DateTime EndDate)[] dates
    )
    {
        var fixture = new Fixture();
        var runs = dates
            .Select(
                item =>
                    fixture
                        .Build<Subscription>()
                        .With(p => p.StartDate, item.StartDate)
                        .With(p => p.EndDate, item.EndDate)
                        .Without(p => p.User)
                        .Without(p => p.Payment)
                        .Create()
            )
            .ToArray();
        return runs;
    }

    private static RunExercise[] CreateRunExercises(DateTime[] dates)
    {
        var fixture = new Fixture();
        var runs = dates
            .Select(
                date =>
                    fixture
                        .Build<RunExercise>()
                        .With(p => p.Date, date)
                        .With(p => p.Result, JsonDocument.Parse("{}"))
                        .Without(p => p.User)
                        .Without(p => p.Exercise)
                        .Create()
            )
            .ToArray();
        return runs;
    }
}

using AutoFixture;
using AutoFixture.AutoMoq;
using Devpull.Controllers;
using Devpull.Course;
using Devpull.DbModels;
using Devpull.Payments;
using Devpull.Payments.PaymentProviders;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace UnitTests.PaymentServiceTests;

[TestClass]
public class CreatePaymentTests
{
    [TestMethod]
    public async Task нет_платежа_создаем()
    {
        var createPaymentMock = new Mock<Func<Task<Payment>>>();
        var cancelPaymentMock = new Mock<Func<Task<Payment>>>();
        var getPaymentStatusMock = new Mock<Func<Task<PaymentStatus>>>();

        var amount = 10;
        var appUser = new AppUser { Email = "test@test.com", };

        await PaymentService.CreatePaymentMethod(
            appUser,
            createPaymentMock.Object,
            cancelPaymentMock.Object,
            getPaymentStatusMock.Object,
            amount,
            CancellationToken.None
        );

        createPaymentMock.Verify(m => m(), Times.Once);
        cancelPaymentMock.Verify(m => m(), Times.Never);
        getPaymentStatusMock.Verify(m => m(), Times.Never);
    }
    
    [TestMethod]
    public async Task есть_платеж_и_отменен_создаем()
    {
        var createPaymentMock = new Mock<Func<Task<Payment>>>();
        var cancelPaymentMock = new Mock<Func<Task<Payment>>>();
        var getPaymentStatusMock = new Mock<Func<Task<PaymentStatus>>>();

        var amount = 10;
        var payment = CreatePayment(PaymentStatus.Canceled, amount);
        var appUser = new AppUser { Email = "test@test.com", Payments =
            [
                payment
            ]
        };

        await PaymentService.CreatePaymentMethod(
            appUser,
            createPaymentMock.Object,
            cancelPaymentMock.Object,
            getPaymentStatusMock.Object,
            amount,
            CancellationToken.None
        );

        createPaymentMock.Verify(m => m(), Times.Once);
        cancelPaymentMock.Verify(m => m(), Times.Never);
        getPaymentStatusMock.Verify(m => m(), Times.Never);
    }

    [TestMethod]
    public async Task есть_платеж_и_завершен_создаем()
    {
        var createPaymentMock = new Mock<Func<Task<Payment>>>();
        var cancelPaymentMock = new Mock<Func<Task<Payment>>>();
        var getPaymentStatusMock = new Mock<Func<Task<PaymentStatus>>>();

        var amount = 10;
        var payment = CreatePayment(PaymentStatus.Succeeded, amount);
        var appUser = new AppUser { Email = "test@test.com", Payments =
            [
                payment
            ]
        };

        await PaymentService.CreatePaymentMethod(
            appUser,
            createPaymentMock.Object,
            cancelPaymentMock.Object,
            getPaymentStatusMock.Object,
            amount,
            CancellationToken.None
        );

        createPaymentMock.Verify(m => m(), Times.Once);
        cancelPaymentMock.Verify(m => m(), Times.Never);
        getPaymentStatusMock.Verify(m => m(), Times.Never);
    }

    [TestMethod]
    [DataRow(PaymentStatus.Succeeded)]
    [DataRow(PaymentStatus.Canceled)]
    public async Task есть_pending_платеж_и_remote_не_pending_синхрон_и_создаем(PaymentStatus remotePaymentStatus)
    {
        var createPaymentMock = new Mock<Func<Task<Payment>>>();
        var cancelPaymentMock = new Mock<Func<Task<Payment>>>();
        var getPaymentStatusMock = new Mock<Func<Task<PaymentStatus>>>();
        getPaymentStatusMock.Setup(m => m()).ReturnsAsync(remotePaymentStatus);

        var amount = 10;
        var payment = CreatePayment(PaymentStatus.Pending, amount);
        var appUser = new AppUser { Email = "test@test.com", Payments =
            [
                payment
            ]
        };

        await PaymentService.CreatePaymentMethod(
            appUser,
            createPaymentMock.Object,
            cancelPaymentMock.Object,
            getPaymentStatusMock.Object,
            amount,
            CancellationToken.None
        );

        createPaymentMock.Verify(m => m(), Times.Once);
        cancelPaymentMock.Verify(m => m(), Times.Never);
        getPaymentStatusMock.Verify(m => m(), Times.Once);
        payment.Status.Should().Be(remotePaymentStatus);
    }

    [TestMethod]
    public async Task есть_pending_платеж_и_remote_pending_возвращаем_платеж()
    {
        var createPaymentMock = new Mock<Func<Task<Payment>>>();
        var cancelPaymentMock = new Mock<Func<Task<Payment>>>();
        var getPaymentStatusMock = new Mock<Func<Task<PaymentStatus>>>();
        getPaymentStatusMock.Setup(m => m()).ReturnsAsync(PaymentStatus.Pending);

        var amount = 10;
        var payment = CreatePayment(PaymentStatus.Pending, amount);
        var appUser = new AppUser { Email = "test@test.com", Payments =
            [
                payment
            ]
        };

        var result = await PaymentService.CreatePaymentMethod(
            appUser,
            createPaymentMock.Object,
            cancelPaymentMock.Object,
            getPaymentStatusMock.Object,
            amount,
            CancellationToken.None
        );

        createPaymentMock.Verify(m => m(), Times.Never);
        cancelPaymentMock.Verify(m => m(), Times.Never);
        getPaymentStatusMock.Verify(m => m(), Times.Once);
        result.Should().Be(payment);
    }

    [TestMethod]
    public async Task есть_pending_платеж_и_remote_pending_но_разные_amount_отменяем_и_создаем()
    {
        var createPaymentMock = new Mock<Func<Task<Payment>>>();
        var cancelPaymentMock = new Mock<Func<Task<Payment>>>();
        var getPaymentStatusMock = new Mock<Func<Task<PaymentStatus>>>();
        getPaymentStatusMock.Setup(m => m()).ReturnsAsync(PaymentStatus.Pending);

        var amount = 10;
        var payment = CreatePayment(PaymentStatus.Pending, amount: 100);
        var appUser = new AppUser { Email = "test@test.com", Payments =
            [
                payment
            ]
        };

        await PaymentService.CreatePaymentMethod(
            appUser,
            createPaymentMock.Object,
            cancelPaymentMock.Object,
            getPaymentStatusMock.Object,
            amount,
            CancellationToken.None
        );

        createPaymentMock.Verify(m => m(), Times.Once);
        cancelPaymentMock.Verify(m => m(), Times.Once);
        getPaymentStatusMock.Verify(m => m(), Times.Once);
    }

    // [TestMethod]
    // public async Task нет_платежа_создаем_на_весь_ендпоинт()
    // {
    //     var fixture = new Fixture().Customize(new AutoMoqCustomization());
    //
    //     var expectedConfirmationUrl = "42";
    //     var myConfiguration = new Dictionary<string, string>
    //     {
    //         { "Yookassa:ReturnBaseUrl", $"https://devpull.courses" },
    //         { "Price:PerMonth", $"100" },
    //         { "Price:PerYear", $"1000" }
    //     };
    //     IConfiguration config = new ConfigurationBuilder()
    //         .AddInMemoryCollection(myConfiguration)
    //         .Build();
    //     fixture.Inject(config);
    //
    //     var cratedPayment = fixture
    //         .Build<Payment>()
    //         .With(p => p.ConfirmationUrl, "42")
    //         .Without(p => p.User)
    //         .Without(p => p.Subscriptions)
    //         .Create();
    //
    //     var stubAuthService = fixture.Freeze<Mock<IAuthService>>();
    //     stubAuthService.Setup(m => m.GetUserIdOrThrow()).Returns("42");
    //
    //     var stubPaymentProvider = fixture.Freeze<Mock<IPaymentProvider>>();
    //     stubPaymentProvider
    //         .Setup(
    //             m =>
    //                 m.CreatePaymentAsync(
    //                     It.IsAny<CreatePaymentOptions>(),
    //                     It.IsAny<CancellationToken>()
    //                 )
    //         )
    //         .ReturnsAsync(cratedPayment);
    //
    //     var mockPaymentRepository = fixture.Freeze<Mock<IPaymentRepository>>();
    //
    //     var sut = fixture.Create<PaymentService>();
    //
    //     var result = await sut.CreatePayment(
    //         new PaymentModel { Type = PaymentType.PerMonth, ReturnUrl = "https://devpull.courses" },
    //         CancellationToken.None
    //     );
    //
    //     result.Should().Be(expectedConfirmationUrl);
    //     stubPaymentProvider.Verify(
    //         m =>
    //             m.CreatePaymentAsync(
    //                 It.IsAny<CreatePaymentOptions>(),
    //                 It.IsAny<CancellationToken>()
    //             ),
    //         Times.Once
    //     );
    //     stubPaymentProvider.Verify(
    //         m => m.GetPaymentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
    //         Times.Never
    //     );
    //     stubPaymentProvider.Verify(
    //         m =>
    //             m.CancelPaymentAsync(
    //                 It.IsAny<string>(),
    //                 It.IsAny<string>(),
    //                 It.IsAny<CancellationToken>()
    //             ),
    //         Times.Never
    //     );
    //     mockPaymentRepository.Verify(
    //         m =>
    //             m.InsertPayment(
    //                 It.IsAny<Payment>(),
    //                 It.IsAny<string>(),
    //                 It.IsAny<CancellationToken>()
    //             ),
    //         Times.Once
    //     );
    // }

    private static Payment CreatePayment(PaymentStatus status, decimal amount)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var payment = fixture
            .Build<Payment>()
            .With(p => p.Status, status)
            .With(p => p.Amount, amount)
            .Without(p => p.User)
            .Without(p => p.Subscriptions)
            .Create();
        return payment;
    }
}

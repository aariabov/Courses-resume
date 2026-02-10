using Devpull.Controllers;
using Devpull.DbModels;
using Devpull.Payments;
using Moq;

namespace UnitTests.PaymentServiceTests;

[TestClass]
public class ProcessNotificationTests
{
    // не success уведомление - исключение
    // нет такого платежа в Юкасса - исключение
    // нет такого платежа в бд - исключение
    // есть и Success - скорей всего повторное, игнор
    // есть и Canceled - исключение, будем разбираться руками с таким платежом
    // есть и Pending, но другая сумма - исключение, будем разбираться руками с таким платежом
    // есть и Pending, та же сумма - успех, обновляем атрибуты, создаем подписку

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
}

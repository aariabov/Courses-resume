using Yandex.Checkout.V3;
using Payment = Devpull.DbModels.Payment;

namespace Devpull.Payments.PaymentProviders;

public record CreatePaymentOptions(
    string UserId,
    string UserEmail,
    string Description,
    decimal Amount,
    PaymentType Type,
    string IdempotenceKey,
    string ReturnUrl
);

public interface IPaymentProvider
{
    Task<DbModels.Payment> CreatePaymentAsync(
        CreatePaymentOptions options,
        CancellationToken cancellationToken
    );
    Task<PaymentStatus> GetPaymentStatusAsync(
        string paymentId,
        CancellationToken cancellationToken
    );
    string GetNotificationId(Notification notification);
    Task<PaymentStatus> CancelPaymentAsync(
        string paymentId,
        string idempotenceKey,
        CancellationToken cancellationToken
    );
    Task<bool> SyncPaymentAttrsIfSuccess(Payment payment, CancellationToken cancellationToken);
    void UpdatePaymentAttrsFromNotification(Payment payment, Notification notification);
}

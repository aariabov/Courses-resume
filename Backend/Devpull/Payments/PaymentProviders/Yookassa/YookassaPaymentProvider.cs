using System.Text.Json;
using Yandex.Checkout.V3;
using Payment = Devpull.DbModels.Payment;
using YandexPaymentStatus = Yandex.Checkout.V3.PaymentStatus;

using Devpull.Common;

namespace Devpull.Payments.PaymentProviders.Yookassa;

public class YookassaPaymentProvider : IPaymentProvider
{
    private readonly AsyncClient _client;

    public YookassaPaymentProvider(AppConfig config)
    {
        var shopId = config.Yookassa.ShopId!;
        var secretKey = config.Yookassa.SecretKey!;
        _client = new Client(shopId, secretKey).MakeAsync();
    }

    public async Task<DbModels.Payment> CreatePaymentAsync(
        CreatePaymentOptions options,
        CancellationToken cancellationToken
    )
    {
        var amount = new Amount { Value = options.Amount, };
        var newPayment = new NewPayment
        {
            Amount = amount,
            Confirmation = new Confirmation
            {
                Type = ConfirmationType.Redirect,
                ReturnUrl = options.ReturnUrl,
            },
            Capture = true,
            Description = options.Description,
            Receipt = new NewReceipt
            {
                Customer = new Customer { Email = options.UserEmail },
                Items =
                [
                    new ReceiptItem
                    {
                        Description = options.Description,
                        Quantity = 1,
                        Amount = amount,
                        VatCode = VatCode.NoVat
                    }
                ]
            }
        };
        var payment = await _client.CreatePaymentAsync(
            newPayment,
            options.IdempotenceKey,
            cancellationToken
        );

        var json = JsonSerializer.Serialize(payment);
        return new DbModels.Payment
        {
            Id = payment.Id,
            UserId = options.UserId,
            Amount = options.Amount,
            Type = options.Type,
            IncomeAmount = null,
            CreatedDate = payment.CreatedAt,
            CapturedDate = null,
            Status = GetPaymentStatus(payment.Status),
            CreateJson = json,
            ConfirmJson = null,
            ConfirmationUrl = payment.Confirmation.ConfirmationUrl
        };
    }

    private static PaymentStatus GetPaymentStatus(YandexPaymentStatus status)
    {
        return status switch
        {
            YandexPaymentStatus.Pending => PaymentStatus.Pending,
            YandexPaymentStatus.Succeeded => PaymentStatus.Succeeded,
            YandexPaymentStatus.Canceled => PaymentStatus.Canceled,
            _ => throw new Exception($"Неподдерживаемый статус {status}")
        };
    }

    public async Task<PaymentStatus> GetPaymentStatusAsync(
        string paymentId,
        CancellationToken cancellationToken
    )
    {
        var payment = await _client.GetPaymentAsync(paymentId, cancellationToken);
        return GetPaymentStatus(payment.Status);
    }

    public string GetNotificationId(Notification notification)
    {
        var payment = GetPaymentFromNotification(notification);
        return payment.Id;
    }

    private static Yandex.Checkout.V3.Payment GetPaymentFromNotification(Notification notification)
    {
        if (notification is PaymentSucceededNotification paymentSucceededNotification)
        {
            return paymentSucceededNotification.Object;
        }

        throw new Exception($"Не поддерживаемый тип уведомления {notification.GetType()}");
    }

    public async Task<PaymentStatus> CancelPaymentAsync(
        string paymentId,
        string idempotenceKey,
        CancellationToken cancellationToken
    )
    {
        var payment = await _client.CancelPaymentAsync(
            paymentId,
            idempotenceKey,
            cancellationToken
        );
        return GetPaymentStatus(payment.Status);
    }

    public void UpdatePaymentAttrsFromNotification(Payment payment, Notification notification)
    {
        var notificationPayment = GetPaymentFromNotification(notification);
        UpdatePaymentAttrs(payment, notificationPayment);
    }

    private static void UpdatePaymentAttrs(Payment payment, Yandex.Checkout.V3.Payment remotePayment)
    {
        var json = JsonSerializer.Serialize(remotePayment);

        payment.IncomeAmount = remotePayment.IncomeAmount.Value;
        payment.Status = GetPaymentStatus(remotePayment.Status);
        payment.CapturedDate = remotePayment.CapturedAt;
        payment.ConfirmJson = json;
    }

    public async Task<bool> SyncPaymentAttrsIfSuccess(
        Payment payment,
        CancellationToken cancellationToken
    )
    {
        var remotePayment = await _client.GetPaymentAsync(payment.Id, cancellationToken);
        if (remotePayment.Status == YandexPaymentStatus.Succeeded)
        {
            UpdatePaymentAttrs(payment, remotePayment);
            return true;
        }

        return false;
    }
}

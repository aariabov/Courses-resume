using System.Text.Json;
using Devpull.Controllers;
using Devpull.Course;
using Devpull.DbModels;
using Devpull.Payments.PaymentProviders;
using Devpull.Users;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Payment = Devpull.DbModels.Payment;

namespace Devpull.Payments;

public class PaymentService
{
    private readonly SubscriptionRepository _subscriptionRepository;
    private readonly Devpull.Common.AppConfig _config;
    private readonly AuthService _authService;
    private readonly PaymentModelValidator _paymentModelValidator;
    private readonly IIdempotenceKeyGenerator _idempotenceKeyGenerator;
    private readonly PaymentRepository _paymentRepository;
    private readonly IUserManagerService _userManagerService;
    private readonly IPaymentProvider _paymentProvider;
    private readonly AppDbContext _db;
    private readonly IUserManagerService _userManager;

    public PaymentService(
        SubscriptionRepository subscriptionRepository,
    Devpull.Common.AppConfig config,
        AuthService authService,
        PaymentModelValidator paymentModelValidator,
        IIdempotenceKeyGenerator idempotenceKeyGenerator,
        PaymentRepository paymentRepository,
        IUserManagerService userManagerService,
        IPaymentProvider paymentProvider,
        AppDbContext db,
        IUserManagerService userManager
    )
    {
        _subscriptionRepository = subscriptionRepository;
    _config = config;
        _authService = authService;
        _paymentModelValidator = paymentModelValidator;
        _idempotenceKeyGenerator = idempotenceKeyGenerator;
        _paymentRepository = paymentRepository;
        _userManagerService = userManagerService;
        _paymentProvider = paymentProvider;
        _db = db;
        _userManager = userManager;
    }

    // public async Task<int> ConfirmPayment(PaymentModel model, CancellationToken cancellationToken)
    // {
    //     await _paymentModelValidator.Validate(model);
    //     return await _paymentRepository.InsertPayment(
    //         Guid.NewGuid(),
    //         model.UserId,
    //         model.Amount,
    //         DateTime.UtcNow,
    //         DateTime.UtcNow.AddDays(30),
    //         cancellationToken
    //     );
    // }

    public async Task<string> CreatePayment(PaymentModel model, CancellationToken cancellationToken)
    {
        await _paymentModelValidator.Validate(model);

        var userId = _authService.GetUserIdOrThrow();
        var user = await _userManagerService.FindByIdAsync(userId);
        var lastPayment = user.Payments.OrderByDescending(p => p.CreatedDate).FirstOrDefault();
        var amount = GetAmount(model.Type);

        var payment = await CreatePaymentMethod(
            user,
            () => CreatePayment(model, user, cancellationToken),
            () => CancelPayment(user, amount, lastPayment!, cancellationToken),
            () => _paymentProvider.GetPaymentStatusAsync(lastPayment!.Id, cancellationToken),
            amount,
            cancellationToken
        );

        await _db.SaveChangesAsync(cancellationToken);
        return payment.ConfirmationUrl;
    }

    public static async Task<Payment> CreatePaymentMethod(
        AppUser user,
        Func<Task<Payment>> createPayment,
        Func<Task> cancelPayment,
        Func<Task<PaymentStatus>> getRemoteStatus,
        decimal amount,
        CancellationToken cancellationToken
    )
    {
        var lastPayment = user.Payments.OrderByDescending(p => p.CreatedDate).FirstOrDefault();
        // нет активного заказа
        if (
            lastPayment is null
            || lastPayment.Status is PaymentStatus.Succeeded or PaymentStatus.Canceled
        )
        {
            return await createPayment();
        }

        if (lastPayment.Status is PaymentStatus.Pending)
        {
            var remoteStatus = await getRemoteStatus();
            if (remoteStatus is not PaymentStatus.Pending)
            {
                lastPayment.Status = remoteStatus;
                return await createPayment();
            }

            // тот же самый платеж
            if (lastPayment.Amount == amount)
            {
                return lastPayment;
            }

            await cancelPayment();

            return await createPayment();
        }

        throw new Exception("Произошла ошибка при создании платежа");
    }

    private static Task CancelPayment(
        AppUser user,
        decimal amount,
        Payment lastPayment,
        CancellationToken cancellationToken
    )
    {
        // почему-то подает ошибка, платеж существует в статусе Pending, но не дает отменить его, не тут, не через апи,
        // возможно починится, когда сделаю вебхуки, ладно пока можно забить
        // var idempotenceKey = _idempotenceKeyGenerator.Generate(user.Id, amount);
        // var status = await _paymentProvider.CancelPaymentAsync(
        //     lastPayment.Id,
        //     idempotenceKey,
        //     cancellationToken
        // );
        lastPayment.Status = PaymentStatus.Canceled;
        return Task.CompletedTask;
    }

    private async Task<Payment> CreatePayment(
        PaymentModel model,
        AppUser user,
        CancellationToken cancellationToken
    )
    {
        var amount = GetAmount(model.Type);
        var description =
            $"Оплата {amount}р. клиентом {user.Email}, дата {DateTime.UtcNow:dd.MM.yyyy HH:mm}";
        var idempotenceKey = _idempotenceKeyGenerator.Generate(user.Id, amount);

        var options = new CreatePaymentOptions(
            UserId: user.Id,
            UserEmail: user.Email,
            Description: description,
            Amount: amount,
            Type: model.Type,
            IdempotenceKey: idempotenceKey,
            ReturnUrl: model.ReturnUrl
        );

        // TODO: сделать обработку ошибок при неудачных запросах
        var payment = await _paymentProvider.CreatePaymentAsync(options, cancellationToken);

        _paymentRepository.InsertPayment(payment, user.Id, cancellationToken);
        return payment;
    }

    private decimal GetAmount(PaymentType type)
    {
        var perMonth = _config.Price.PerMonth;
        var perYear = _config.Price.PerYear;

        var amount = type switch
        {
            PaymentType.PerMonth => perMonth,
            PaymentType.PerYear => perYear,
            _ => throw new ArgumentException("Invalid payment type")
        };
        return amount;
    }

    public Task<PricesInfo> GetPrices()
    {
        var perMonth = _config.Price.PerMonth;
        var perYear = _config.Price.PerYear;
        return Task.FromResult(new PricesInfo { PerMonth = perMonth, PerYear = perYear });
    }

    public async Task<int> ProcessNotification(
        Yandex.Checkout.V3.Notification notification,
        CancellationToken cancellationToken
    )
    {
        // ЮКасса дает мало времени на обработку хука, потом отменяет запрос
        // если надо подебажить, то делать это быстро, либо использовать синхронные методы, либо другой cancellationToken

        var paymentId = _paymentProvider.GetNotificationId(notification);
        var remotePaymentStatus = await _paymentProvider.GetPaymentStatusAsync(
            paymentId,
            cancellationToken
        );
        var payment = await _paymentRepository.GetById(paymentId, cancellationToken);

        if (
            payment.Status is PaymentStatus.Pending
            && remotePaymentStatus is PaymentStatus.Succeeded
        )
        {
            _paymentProvider.UpdatePaymentAttrsFromNotification(payment, notification);

            var user = await _userManagerService.FindByIdAsync(payment.UserId);
            user.CreateSubscription(payment.Id, payment.Type);

            await SaveChangesIgnoreUnique(cancellationToken);
            return 1;
        }

        throw new Exception(
            $"Невалидное уведомление статус в БД не равен {PaymentStatus.Pending}, либо статус у провайдера не равен {PaymentStatus.Succeeded}"
        );
    }

    public async Task<bool> IsLastPaymentSuccess(CancellationToken cancellationToken)
    {
        Console.WriteLine("IsLastPaymentSuccess FIRED");
        var userId = _authService.GetUserIdOrThrow();
        var user = await _userManager.FindByIdAsync(userId);
        var lastPayment = user.GetLastPayment();
        if (lastPayment is null)
        {
            return false;
        }

        if (lastPayment.Status is PaymentStatus.Succeeded)
        {
            return true;
        }

        var isPaymentSuccess = await _paymentProvider.SyncPaymentAttrsIfSuccess(
            lastPayment,
            cancellationToken
        );
        if (!isPaymentSuccess)
        {
            return false;
        }

        user.CreateSubscription(lastPayment.Id, lastPayment.Type);
        await SaveChangesIgnoreUnique(cancellationToken);

        return true;
    }

    private async Task SaveChangesIgnoreUnique(CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException
                    is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
            )
        {
            // В состоянии гонки может быть, что подписка уже создана, поэтому просто игнорируем
        }
    }
}

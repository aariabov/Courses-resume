using System.ComponentModel.DataAnnotations;
using Devpull.Controllers;
using Devpull.Course;

namespace Devpull.Payments;

public enum PaymentType
{
    PerMonth = 1,
    PerYear = 2
}

public class PaymentModel
{
    [EnumDataType(typeof(PaymentType), ErrorMessage = "Недопустимый тип оплаты")]
    public PaymentType Type { get; set; }

    [Required(ErrorMessage = "ReturnUrl обязателен")]
    public required string ReturnUrl { get; set; }
}

public class PaymentModelValidator : ValidatorBase<PaymentModel>
{
    private readonly AuthService _authService;
    private readonly Devpull.Common.AppConfig _config;

    public PaymentModelValidator(AuthService authService, Devpull.Common.AppConfig config)
    {
        _authService = authService;
        _config = config;
    }

    protected override Task ValidateAsync(ValidationErrors res, PaymentModel model)
    {
        var userId = _authService.GetUserIdOrThrow();

        var returnBaseUrl = _config.Yookassa.ReturnBaseUrl!;
        // if (!model.ReturnUrl.StartsWith(returnBaseUrl, StringComparison.OrdinalIgnoreCase))
        // {
        //     res.Add(
        //         nameof(model.ReturnUrl),
        //         $"{nameof(model.ReturnUrl)} должен начинаться с {returnBaseUrl}"
        //     );
        // }

        // TODO: возможно, стоит защититься от повторных запросов. Хотя ЮКасса использует idempotenceKey, который живет в течении суток.
        // Если хранить ответы от Юкассы в бд и при повторном запросы отдавать ту же ссылку, то она может протухнуть. Опять же придется спрашивать у Юкассы валидна ли она.
        // Конечно можно реагировать на webhook о смене статуса заказа и удалять его из бд, но можно продолбать его, и клиент не сможет заплатить.
        return Task.CompletedTask;
    }
}

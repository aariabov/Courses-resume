using Devpull.Controllers;
using Devpull.Payments;

namespace Devpull.DbModels;

public class Payment
{
    public required string Id { get; init; }
    public required string UserId { get; init; }
    public required decimal Amount { get; set; }

    /// <summary>Сумма после комиссии</summary>
    public required decimal? IncomeAmount { get; set; }
    public required string ConfirmationUrl { get; init; }
    public required PaymentStatus Status { get; set; }
    public required PaymentType Type { get; init; }
    public required DateTime CreatedDate { get; init; }

    /// <summary>Когда платеж был принят</summary>
    public required DateTime? CapturedDate { get; set; }
    public required string CreateJson { get; init; }
    public required string? ConfirmJson { get; set; }

    public virtual AppUser User { get; set; } = null!;
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}

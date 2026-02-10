using Devpull.Controllers;

namespace Devpull.DbModels;

public class Subscription
{
    public Guid Id { get; set; }
    public required string UserId { get; init; }
    public required string PaymentId { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }

    public virtual AppUser User { get; set; } = null!;
    public virtual Payment Payment { get; set; } = null!;
}

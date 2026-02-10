using Devpull.Payments;

namespace Devpull.Controllers.Models;

public class UserInfo
{
    public required string Email { get; init; }
    public required SubscriptionInfo[] Subscriptions { get; init; } = [];
}

public class SubscriptionInfo
{
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required decimal Amount { get; init; }
    public required PaymentType Type { get; init; }
    public bool IsActive {
        get
        {
            var now = DateTime.UtcNow;
            return StartDate < now && now < EndDate;
        }
    }

    public int DaysLeft => EndDate.Date.Subtract(DateTime.UtcNow.Date).Days;
    public SubscriptionStatus Status 
    {
        get
        {
            var now = DateTime.UtcNow;
            if (IsActive)
            {
                return SubscriptionStatus.Active;
            }

            return EndDate < now ? SubscriptionStatus.Expired : SubscriptionStatus.Pending;
        }
    }
}

public enum SubscriptionStatus
{
    Expired = 1,
    Active = 2,
    Pending = 3
}

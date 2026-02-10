using Devpull.DbModels;

namespace Devpull.Payments;

public class SubscriptionRepository
{
    private readonly AppDbContext _db;

    public SubscriptionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> InsertSubscription(
        Guid id,
        string userId,
        string paymentId,
        int amount,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken
    )
    {
        var subscription = new Subscription
        {
            Id = id,
            UserId = userId,
            PaymentId = paymentId,
            StartDate = startDate,
            EndDate = endDate
        };
        _db.Subscriptions.Add(subscription);
        return await _db.SaveChangesAsync(cancellationToken);
    }
}

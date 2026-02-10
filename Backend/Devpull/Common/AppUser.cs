using Devpull.DbModels;
using Devpull.Payments;
using Microsoft.AspNetCore.Identity;

namespace Devpull.Controllers;

public class AppUser : IdentityUser
{
    public new required string Email { get; set; }
    public string? EmailConfirmationCode { get; set; }
    public DateTime? EmailConfirmationCodeExpiry { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<RunExercise> RunExercises { get; set; } = new List<RunExercise>();

    public Payment? GetLastPayment()
    {
        return Payments.OrderBy(p => p.CreatedDate).LastOrDefault();
    }

    public (bool CanRun, DateTime? FromDate) CanUserRunExercise(int freeRunsPerDay)
    {
        var now = DateTime.UtcNow;
        var userHasSubscription = Subscriptions.Any(p => p.StartDate < now && p.EndDate > now);
        if (userHasSubscription)
        {
            return (true, null);
        }

        var lastRuns = RunExercises.OrderByDescending(r => r.Date).Take(freeRunsPerDay).ToArray();
        if (lastRuns.Length < freeRunsPerDay)
        {
            return (true, null);
        }

        var startDate = lastRuns.Min(r => r.Date);
        var endDate = startDate.AddDays(1);
        if (endDate < now)
        {
            return (true, null);
        }

        return (false, endDate);
    }

    public void CreateSubscription(string paymentId, PaymentType paymentType)
    {
        var now = DateTime.UtcNow;
        var lastSubscription = Subscriptions
            .Where(p => p.EndDate > now)
            .OrderBy(s => s.EndDate)
            .LastOrDefault();

        var startDate = lastSubscription?.EndDate ?? now;
        var endDate = GetEndDate(startDate, paymentType);

        var newSubscription = new Subscription
        {
            StartDate = startDate,
            EndDate = endDate,
            UserId = Id,
            PaymentId = paymentId,
        };
        Subscriptions.Add(newSubscription);
    }

    public DateTime GetEndDate(DateTime startDate, PaymentType type)
    {
        if (type == PaymentType.PerMonth)
        {
            return startDate.AddDays(31);
        }

        if (type == PaymentType.PerYear)
        {
            return startDate.AddYears(1);
        }

        throw new Exception($"Не поддерживаемый PaymentType {type}");
    }
}

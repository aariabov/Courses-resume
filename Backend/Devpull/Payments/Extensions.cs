using Devpull.Payments.PaymentProviders;
using Devpull.Payments.PaymentProviders.Yookassa;

namespace Devpull.Payments;

public static class Extensions
{
    public static void AddPayment(this IServiceCollection services)
    {
        services.AddScoped<SubscriptionRepository>();
        services.AddScoped<PaymentRepository>();
        services.AddScoped<PaymentService>();
        services.AddScoped<IPaymentProvider, YookassaPaymentProvider>();
        services.AddScoped<PaymentModelValidator>();
    }
}

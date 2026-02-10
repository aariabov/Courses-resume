using System.Text.Json;
using Devpull.Controllers;
using Devpull.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Devpull.Payments;

public class PaymentRepository
{
    private readonly AppDbContext _db;

    public PaymentRepository(AppDbContext db)
    {
        _db = db;
    }

    public bool InsertPayment(Payment payment, string userId, CancellationToken cancellationToken)
    {
        _db.Payments.Add(payment);
        return true;
    }

    public async Task<Payment> GetById(string paymentId, CancellationToken cancellationToken)
    {
        return await _db.Payments.FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken)
            ?? throw new NotFoundException(paymentId);
    }
}

using Application.Interfaces.Repositories;
using Core.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories;

public class PaymentRepository (AppDbContext context) : IPaymentRepository
{
    public async Task AddPayment(Payments payment)
    {
        await context.Payments.AddAsync(payment);
    }

    public async Task<Payments> GetPaymentByReference(string reference)
    {
        var payment = await context.Payments.FirstOrDefaultAsync(payment => payment.TransactionReference == reference);
        return payment;
    }

    public async Task<Payments> GetPaymentByBookingId(long bookingId)
    {
        var payment = await context.Payments.FirstOrDefaultAsync(payment => payment.BookingId == bookingId);
        return payment;
    }

    public async Task UpdatePayment(Payments payment)
    {
        context.Payments.Update(payment);
        await context.SaveChangesAsync();
    }
}
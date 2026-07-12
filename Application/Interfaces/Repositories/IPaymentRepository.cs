using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IPaymentRepository
{
    Task AddPayment(Payments payment);
    Task<Payments> GetPaymentByReference(string reference);
    Task UpdatePayment(Payments payment);
}
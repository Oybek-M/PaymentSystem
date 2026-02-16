using PaymentSystem.Domain.Entities;

namespace PaymentSystem.DAL.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByPhoneNumberAsync(string phoneNumber);
}

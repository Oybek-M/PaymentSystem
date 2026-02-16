using Microsoft.EntityFrameworkCore;
using PaymentSystem.DAL.Data;
using PaymentSystem.Domain.Entities;

namespace PaymentSystem.DAL.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Payment>> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _dbSet
            .Where(p => p.PhoneNumber == phoneNumber)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}

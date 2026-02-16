using Microsoft.EntityFrameworkCore;
using PaymentSystem.DAL.Data;
using PaymentSystem.Domain.Entities;

namespace PaymentSystem.DAL.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }
}

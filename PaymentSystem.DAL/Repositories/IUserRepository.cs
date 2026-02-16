using PaymentSystem.Domain.Entities;

namespace PaymentSystem.DAL.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
}

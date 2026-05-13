using SpendSmart.Auth.API.Models;

namespace SpendSmart.Auth.API.Repositories;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task DeleteAllExceptAsync(int currentAdminId);
}

using Microsoft.EntityFrameworkCore;
using SpendSmart.Auth.API.Data;
using SpendSmart.Auth.API.Models;

namespace SpendSmart.Auth.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> FindByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllExceptAsync(int currentAdminId)
    {
        var usersToDelete = await _context.Users
            .Where(u => u.UserId != currentAdminId && u.Role != "Admin")
            .ToListAsync();
        _context.Users.RemoveRange(usersToDelete);
        await _context.SaveChangesAsync();
    }
}

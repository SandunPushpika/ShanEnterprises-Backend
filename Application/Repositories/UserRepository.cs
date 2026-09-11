using Application.Interfaces.Repositories;
using Core.DTOs.Response;
using Core.DTOs.Request.Customer;
using Core.Entities;
using Core.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    
    public async Task<User> AddUserAsync(User user)
    {
        var newUser = await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return newUser.Entity;
    }

   
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<User> UpdateUserAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();

        return user;
    }
    public async Task<(IReadOnlyCollection<User> Users, int Total)> GetCustomersAsync(CustomerSearchRequest request)
    {
        var query = context.Users.Where(x => x.Role == UserRole.CUSTOMER);
        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(x =>
                x.FirstName.ToLower().Contains(term) ||
                x.LastName.ToLower().Contains(term) ||
                x.Email.ToLower().Contains(term) ||
                (x.PhoneNumber != null && x.PhoneNumber.Contains(term)) ||
                (x.NicPassportNumber != null && x.NicPassportNumber.ToLower().Contains(term)));
        }
        var total = await query.CountAsync();
        var users = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return (users, total);
    }
}
    

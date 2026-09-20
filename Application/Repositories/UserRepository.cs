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
            if (request.Status.Value == UserStatus.INACTIVE)
            {
                query = query.Where(x => x.Status == UserStatus.INACTIVE || x.Status == UserStatus.SUSPENDED);
            }
            else
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }
        }
        
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.FirstName, $"%{term}%") ||
                EF.Functions.ILike(x.LastName, $"%{term}%") ||
                EF.Functions.ILike(x.Email, $"%{term}%") ||
                (x.PhoneNumber != null && EF.Functions.ILike(x.PhoneNumber, $"%{term}%")) ||
                (x.NicPassportNumber != null && EF.Functions.ILike(x.NicPassportNumber, $"%{term}%")));
        }
        var total = await query.CountAsync();
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 8 : request.PageSize;
        var users = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (users, total);
    }

    public async Task<CustomerStatsResponse> GetCustomerStatsAsync()
    {
        var baseQuery = context.Users.Where(x => x.Role == UserRole.CUSTOMER);

        return new CustomerStatsResponse
        {
            Total = await baseQuery.CountAsync(),
            Active = await baseQuery.CountAsync(x => x.Status == UserStatus.ACTIVE),
            Inactive = await baseQuery.CountAsync(x => x.Status == UserStatus.INACTIVE || x.Status == UserStatus.SUSPENDED),
            Verified = await baseQuery.CountAsync(x => x.EmailVerified)
        };
    }
}
    

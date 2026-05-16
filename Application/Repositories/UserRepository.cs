using Application.Interfaces.Repositories;
using Core.Entities;
using Infrastructure.Database;

namespace Application.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task AddUser(User user)
    {
        await context.Users.AddAsync(user);
    }
}
using Core.DTOs.Response;
using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(int id);
    Task<User> UpdateUserAsync(User user);
}
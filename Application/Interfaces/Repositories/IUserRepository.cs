using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
     
    Task AddUserAsync(User user);

    
    Task<User?> GetUserByEmailAsync(string email);

    // get user by id
    Task<User?> GetUserByIdAsync(int id);

    // update user
    Task<User> UpdateUserAsync(User user);
}
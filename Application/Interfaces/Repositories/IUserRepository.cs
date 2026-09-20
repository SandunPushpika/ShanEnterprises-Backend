using Core.DTOs.Response;
using Core.Entities;
using Core.DTOs.Request.Customer;
namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> AddUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(int id);
    Task<User> UpdateUserAsync(User user);
    Task<(IReadOnlyCollection<User> Users, int Total)> GetCustomersAsync(CustomerSearchRequest request);
    Task<CustomerStatsResponse> GetCustomerStatsAsync();
}
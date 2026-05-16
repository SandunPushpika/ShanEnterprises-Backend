using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddUser(User user);
}
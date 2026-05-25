using Core.Entities;

namespace Application.Interfaces.Services;

public interface IContextService
{
    Task<User> GetUser();
}
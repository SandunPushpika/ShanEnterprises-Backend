using Core.Entities;

namespace Core.Interfaces;

public interface IApplicationContext
{
    User GetUser();
}
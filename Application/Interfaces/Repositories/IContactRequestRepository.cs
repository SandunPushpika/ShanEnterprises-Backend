using Core.DTOs.Request.Contact;
using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IContactRequestRepository
{
    Task<ContactRequest> AddAsync(ContactRequest request);
    Task UpdateAsync(ContactRequest request);
    Task<ContactRequest?> GetByIdAsync(int id);
    Task<(IReadOnlyCollection<ContactRequest> Items, int Total)> GetAllAsync(ContactRequestSearchDto search);
}

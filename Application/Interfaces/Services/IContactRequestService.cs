using Core.DTOs.Request.Contact;
using Core.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IContactRequestService
{
    Task SubmitContactRequestAsync(ContactRequestCreateDto dto);
    Task<SearchResponse<ContactRequestResponse>> GetAllContactRequestsAsync(ContactRequestSearchDto search);
    Task<ContactRequestResponse> GetContactRequestByIdAsync(int id);
    Task UpdateContactRequestStatusAsync(int id, UpdateContactStatusDto dto);
}

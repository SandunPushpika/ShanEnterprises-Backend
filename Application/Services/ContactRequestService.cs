using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Core.DTOs.Request.Contact;
using Core.DTOs.Response;
using Core.Entities;
using Core.Exceptions;

namespace Application.Services;

public class ContactRequestService : IContactRequestService
{
    private readonly IContactRequestRepository _repository;
    private readonly IContextService _context;

    public ContactRequestService(IContactRequestRepository repository, IContextService context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task SubmitContactRequestAsync(ContactRequestCreateDto dto)
    {
        var user = await _context.GetUser();
        var contactRequest = new ContactRequest
        {
            Name = dto.Name,
            Email = dto.Email,
            Subject = dto.Subject,
            Message = dto.Message,
            UserId = user?.Id,
            Status = "NEW",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _repository.AddAsync(contactRequest);
    }

    public async Task<SearchResponse<ContactRequestResponse>> GetAllContactRequestsAsync(ContactRequestSearchDto search)
    {
        var (items, total) = await _repository.GetAllAsync(search);
        var responses = items.Select(c => MapToResponse(c)).ToList().AsReadOnly();
        return new SearchResponse<ContactRequestResponse>
        {
            Data = responses,
            Total = total,
            PageNumber = search.PageNumber,
            PageSize = search.PageSize
        };
    }

    public async Task<ContactRequestResponse> GetContactRequestByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) throw new NotFoundException($"Contact request {id} not found.");
        return MapToResponse(item);
    }

    public async Task UpdateContactRequestStatusAsync(int id, UpdateContactStatusDto dto)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) throw new NotFoundException($"Contact request {id} not found.");
        item.Status = dto.Status;
        item.AdminNotes = dto.AdminNotes;
        item.UpdatedAt = DateTime.UtcNow;
        item.CreatedAt = DateTime.SpecifyKind(item.CreatedAt, DateTimeKind.Utc);
        await _repository.UpdateAsync(item);
    }

    private static ContactRequestResponse MapToResponse(ContactRequest c) => new()
    {
        Id = c.Id,
        UserId = c.UserId,
        UserName = c.User != null ? $"{c.User.FirstName} {c.User.LastName}" : null,
        Name = c.Name,
        Email = c.Email,
        Subject = c.Subject,
        Message = c.Message,
        Status = c.Status,
        AdminNotes = c.AdminNotes,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}

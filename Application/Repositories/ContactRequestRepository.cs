using Application.Interfaces.Repositories;
using Core.DTOs.Request.Contact;
using Core.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories;

public class ContactRequestRepository(AppDbContext context) : IContactRequestRepository
{
    public async Task<ContactRequest> AddAsync(ContactRequest request)
    {
        var result = await context.ContactRequests.AddAsync(request);
        await context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task UpdateAsync(ContactRequest request)
    {
        context.ContactRequests.Update(request);
        await context.SaveChangesAsync();
    }

    public async Task<ContactRequest?> GetByIdAsync(int id)
    {
        return await context.ContactRequests
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(IReadOnlyCollection<ContactRequest> Items, int Total)> GetAllAsync(ContactRequestSearchDto search)
    {
        var query = context.ContactRequests
            .Include(c => c.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search.Status))
            query = query.Where(c => c.Status == search.Status);

        if (!string.IsNullOrWhiteSpace(search.Search))
            query = query.Where(c =>
                EF.Functions.ILike(c.Name, $"%{search.Search}%") ||
                EF.Functions.ILike(c.Email, $"%{search.Search}%") ||
                EF.Functions.ILike(c.Subject, $"%{search.Search}%"));

        var total = await query.CountAsync();
        var pn = search.PageNumber < 1 ? 1 : search.PageNumber;
        var ps = search.PageSize < 1 ? 10 : search.PageSize;
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pn - 1) * ps)
            .Take(ps)
            .ToListAsync();

        return (items, total);
    }
}

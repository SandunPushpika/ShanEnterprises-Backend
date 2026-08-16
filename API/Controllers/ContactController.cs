using Application.Interfaces.Services;
using Core.DTOs.Request.Contact;
using Core.DTOs.Response;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShanEnterprises.Attributes;

namespace ShanEnterprises.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactRequestService _contactService;

    public ContactController(IContactRequestService contactService)
    {
        _contactService = contactService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> SubmitContactRequest([FromBody] ContactRequestCreateDto dto)
    {
        await _contactService.SubmitContactRequestAsync(dto);
        return new ApiResponse("Your message has been received. We'll get back to you soon!");
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpPost("all")]
    public async Task<ActionResult<ApiResponse>> GetAllContactRequests([FromBody] ContactRequestSearchDto search)
    {
        var result = await _contactService.GetAllContactRequestsAsync(search);
        return new ApiResponse(data: result);
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetContactRequest(int id)
    {
        var result = await _contactService.GetContactRequestByIdAsync(id);
        return new ApiResponse(data: result);
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse>> UpdateStatus(int id, [FromBody] UpdateContactStatusDto dto)
    {
        await _contactService.UpdateContactRequestStatusAsync(id, dto);
        return new ApiResponse("Status updated successfully.");
    }
}

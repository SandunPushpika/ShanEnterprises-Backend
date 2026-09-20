using Application.Interfaces.Services;
using Core.DTOs.Request.Auth;
using Core.DTOs.Request.User;
using Core.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Enums;
using Core.DTOs.Request.Customer;
using ShanEnterprises.Attributes;

namespace ShanEnterprises.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse>> GetProfile()
    {
        var profile = await _userService.GetCurrentUserProfileAsync();
        return new ApiResponse(data: profile);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse>> UpdateProfile([FromBody] UserProfileUpdateRequest request)
    {
        var updatedProfile = await _userService.UpdateCurrentUserProfileAsync(request);
        return new ApiResponse("Profile updated successfully", data: updatedProfile);
    }

    [HttpPost("profile-image")]
    public async Task<ActionResult<ApiResponse>> UploadProfileImage(IFormFile file)
    {
        var updatedProfile = await _userService.UpdateProfileImageAsync(file);
        return new ApiResponse("Profile image updated successfully", data: updatedProfile);
    }

    [HttpDelete("profile-image")]
    public async Task<ActionResult<ApiResponse>> RemoveProfileImage()
    {
        var updatedProfile = await _userService.RemoveProfileImageAsync();
        return new ApiResponse("Profile image removed successfully", data: updatedProfile);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return new ApiResponse("User not found", success: false);

        return new ApiResponse("", data: user);
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse>> UpdateUserDetails([FromBody] UserUpdateRequest request)
    {
        var updatedUser = await _userService.UpdateUserAsync(request);

        return new ApiResponse("User updated successfully", data: updatedUser);
    }
    
    [CustomAuthorize(UserRole.ADMIN)]
    [HttpPost("customers/search")]
    public async Task<ActionResult<ApiResponse>> GetCustomers([FromBody] CustomerSearchRequest request)
    {
        var result = await _userService.GetCustomersAsync(request);
        return new ApiResponse(data: result);
    }
    
    [CustomAuthorize(UserRole.ADMIN)]
    [HttpGet("customers/stats")]
    public async Task<ActionResult<ApiResponse>> GetCustomerStats()
    {
        var stats = await _userService.GetCustomerStatsAsync();
        return new ApiResponse(data: stats);
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpPut("{userId}/status")]
    public async Task<ActionResult<ApiResponse>> UpdateCustomerStatus(long userId, [FromBody] CustomerStatusUpdateRequest request)
    {
        var updated = await _userService.UpdateUserStatusAsync(userId, request.Status);
        return new ApiResponse("Customer status updated successfully", data: updated);
    }
}
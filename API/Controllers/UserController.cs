using Application.Interfaces.Services;
using Core.DTOs.Request.Auth;
using Core.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace ShanEnterprises.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

 
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return new ApiResponse("User not found",success: false);

        return Ok(new ApiResponse("", data: user));
    }

     
    [HttpPost("updateUserDetails")]
    public async Task<IActionResult> UpdateUserDetails([FromBody] UserUpdateRequest request)
    {
        var updatedUser = await _userService.UpdateUserAsync(request);

        return Ok(new ApiResponse("User updated successfully", data: updatedUser));
    }
}
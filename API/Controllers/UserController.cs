using Application.Interfaces.Services;
using Core.DTOs.Request;
using Core.DTOs.Request.Auth;
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

 
    [HttpGet("getUserById/{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound("User not found");

        return Ok(user);
    }

     
    [HttpPost("updateUserDetails")]
    public async Task<IActionResult> UpdateUserDetails([FromBody] UserUpdateRequest request)
    {
        if (request == null)
            return BadRequest("Invalid request");

        var updatedUser = await _userService.UpdateUserAsync(request);

        return Ok(updatedUser);
    }
}
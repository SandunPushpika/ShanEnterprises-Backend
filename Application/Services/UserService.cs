using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Auth;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using Core.DTOs.Request.Customer;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IContextService _contextService;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IContextService contextService, IMapper mapper)
    {
        _userRepository = userRepository;
        _contextService = contextService;
        _mapper = mapper;
    }
    
    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        var loggedUser = await _contextService.GetUser();
        if (loggedUser.Id != id && loggedUser.Role != UserRole.ADMIN)
            return null;
                
        var user = await _userRepository.GetUserByIdAsync(id);

        return user == null ? null : _mapper.Map<UserResponse>(user);
    }
    
    public async Task<UserResponse> UpdateUserAsync(UserUpdateRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(request.Id);

        if (user == null)
            throw new Exception("User not found");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.UpdatedAt = DateTime.UtcNow;
        user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);

        var updatedUser = await _userRepository.UpdateUserAsync(user);

        return _mapper.Map<UserResponse>(user);
    }
    
    public async Task<SearchResponse<UserResponse>> GetCustomersAsync(CustomerSearchRequest request)
    {
        var (users, total) = await _userRepository.GetCustomersAsync(request);
        return new SearchResponse<UserResponse>
        {
            Data = _mapper.Map<List<UserResponse>>(users),
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<UserResponse> UpdateUserStatusAsync(long userId, UserStatus status)
    {
        var user = await _userRepository.GetUserByIdAsync((int)userId);
        if (user == null)
            throw new Exception("Customer not found");
        user.Status = status;
        user.UpdatedAt = DateTime.UtcNow;
        user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);
        await _userRepository.UpdateUserAsync(user);
        return _mapper.Map<UserResponse>(user);
    }

}
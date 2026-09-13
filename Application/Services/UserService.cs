using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Auth;
using Core.DTOs.Request.User;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Helpers;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Core.DTOs.Request.Customer;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IContextService _contextService;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;
    private readonly AppSettings _appSettings;

    public UserService(
        IUserRepository userRepository,
        IContextService contextService,
        IMapper mapper,
        IStorageService storageService,
        IOptions<AppSettings> appSettings)
    {
        _userRepository = userRepository;
        _contextService = contextService;
        _mapper = mapper;
        _storageService = storageService;
        _appSettings = appSettings.Value;
    }
    
    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        var loggedUser = await _contextService.GetUser();
        if (loggedUser.Id != id && loggedUser.Role != UserRole.ADMIN)
            return null;
                
        var user = await _userRepository.GetUserByIdAsync(id);

        return user == null ? null : _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> GetCurrentUserProfileAsync()
    {
        var loggedUser = await _contextService.GetUser();
        var user = await _userRepository.GetUserByIdAsync((int)loggedUser.Id);

        if (user == null)
            throw new NotFoundException("User not found");

        return _mapper.Map<UserResponse>(user);
    }
    
    public async Task<UserResponse> UpdateCurrentUserProfileAsync(UserProfileUpdateRequest request)
    {
        var loggedUser = await _contextService.GetUser();
        var user = await _userRepository.GetUserByIdAsync((int)loggedUser.Id);

        if (user == null)
            throw new NotFoundException("User not found");

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        user.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        user.City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim();
        user.NicPassportNumber = string.IsNullOrWhiteSpace(request.NicPassportNumber) ? null : request.NicPassportNumber.Trim();

        if (request.ProfileImageUrl != null)
        {
            user.ProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl.Trim();
        }

        user.UpdatedAt = DateTime.UtcNow;
        user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);

        var updatedUser = await _userRepository.UpdateUserAsync(user);

        return _mapper.Map<UserResponse>(updatedUser);
    }

    public async Task<UserResponse> UpdateProfileImageAsync(IFormFile file)
    {
        var loggedUser = await _contextService.GetUser();
        var user = await _userRepository.GetUserByIdAsync((int)loggedUser.Id);

        if (user == null)
            throw new NotFoundException("User not found");

        if (file == null || file.Length == 0)
            throw new FailedOperationException("Please provide a valid image file.");

        if (file.Length > 5 * 1024 * 1024)
            throw new FailedOperationException("Image file size cannot exceed 5MB.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension) || !file.ContentType.StartsWith("image/"))
            throw new FailedOperationException("Invalid image file format. Only JPEG, PNG, WEBP, and GIF are allowed.");

        var fileName = $"profile_{user.Id}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{extension}";
        var imageUrl = await _storageService.UploadBlobAsync(file, BlobType.PROFILE, _appSettings.BlobConnectionString, fileName);

        user.ProfileImageUrl = imageUrl;
        user.UpdatedAt = DateTime.UtcNow;
        user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);

        var updatedUser = await _userRepository.UpdateUserAsync(user);

        return _mapper.Map<UserResponse>(updatedUser);
    }

    public async Task<UserResponse> RemoveProfileImageAsync()
    {
        var loggedUser = await _contextService.GetUser();
        var user = await _userRepository.GetUserByIdAsync((int)loggedUser.Id);

        if (user == null)
            throw new NotFoundException("User not found");

        user.ProfileImageUrl = null;
        user.UpdatedAt = DateTime.UtcNow;
        user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);

        var updatedUser = await _userRepository.UpdateUserAsync(user);

        return _mapper.Map<UserResponse>(updatedUser);
    }
    
    public async Task<UserResponse> UpdateUserAsync(UserUpdateRequest request)
    {
        var loggedUser = await _contextService.GetUser();
        if (loggedUser.Id != request.Id && loggedUser.Role != UserRole.ADMIN)
            throw new UnauthorizedAccessException("You are not authorized to update this profile.");

        var user = await _userRepository.GetUserByIdAsync(request.Id);

        if (user == null)
            throw new NotFoundException("User not found");

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
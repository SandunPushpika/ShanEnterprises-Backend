using System.Security.Authentication;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Auth;
using Core.DTOs.Request.Other;
using Core.DTOs.Response.Auth;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Exceptions.Auth;
using Core.Helpers;
using Core.Helpers.Templates;
using Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IVerificationCodeRepository _verificationCodeRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly AppSettings _appSettings;
    private readonly IOAuthService _oAuthService;

    public AuthService(IUserRepository userRepository, 
        IVerificationCodeRepository verificationCodeRepository, 
        IEmailService emailService, 
        IMapper mapper, 
        IOptions<AppSettings> appSettings,
        [FromKeyedServices(OAuthProvider.GOOGLE)] IOAuthService oAuthService)
    {
        _userRepository = userRepository;
        _verificationCodeRepository = verificationCodeRepository;
        _emailService = emailService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
        _oAuthService = oAuthService;
    }
    
    public async Task<User> RegisterUser(CreateUserRequest request, bool socialMediaRequest = false)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
        if (existingUser != null)
            throw new UserAlreadyExistsException(request.Email);

        if (request is { Role: UserRole.ADMIN, KeyCode: not null } && !request.KeyCode.Equals(_appSettings.AdminKeyCode))
            throw new Exception("Invalid Admin Key!");
        
        var user = _mapper.Map<User>(request);
        user.CreatedAt = DateTime.UtcNow;
        user.PasswordHash = PasswordHasher.HashPassword(request.Password);
        user.EmailVerified = request.Role == UserRole.ADMIN || socialMediaRequest;
        user.Status = UserStatus.ACTIVE;
        
        var newUser = await _userRepository.AddUserAsync(user);
        
        if(request.Role != UserRole.ADMIN && !socialMediaRequest)
            await SendVerificationEmail((int)newUser.Id, user.Email, user.FirstName);
        
        return newUser;
    }

    public async Task<LoginResponse> LoginUser(LoginRequest request)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
        if (existingUser == null)
            throw new InvalidCredentialsException();

        if (existingUser.Status != UserStatus.ACTIVE)
            throw new InvalidCredentialException("User is not active!");
        
        if(!PasswordHasher.VerifyPassword(request.Password, existingUser.PasswordHash))
            throw new InvalidCredentialsException();

        return GetLoginResponse(existingUser);
    }

    public async Task<LoginResponse> RefreshToken(string refreshToken)
    {
        var userId = JwtHelper.GetUserIdFromToken(refreshToken, _appSettings.JwtSettings);
        if(userId == null)
            throw new UnauthorizedUserException();

        var user = await _userRepository.GetUserByIdAsync(int.Parse(userId));
        if(user == null)
            throw new UnauthorizedUserException();
        
        return GetLoginResponse(user);
    }

    public async Task VerifyCode(string code)
    {
        var verificationCode = await _verificationCodeRepository.GetVerificationCodeByCode(code);
        if(verificationCode == null)
            throw new VerificationCodeException("Invalid code");
        
        if(verificationCode.IsUsed || verificationCode.ExpiresAt < DateTime.UtcNow)
            throw new VerificationCodeException("Code Expired");
        
        var user = await _userRepository.GetUserByIdAsync(verificationCode.UserId);
        if (user == null)
            throw new NotFoundException("User not found");
        
        user.EmailVerified = true;
        user.UpdatedAt = DateTime.UtcNow;
        user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);
        
        await _userRepository.UpdateUserAsync(user);
    }

    public async Task ResendVerificationCode(string email)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(email.ToLower());
        if (existingUser == null)
            throw new NotFoundException("User not found");
        
        if(existingUser.EmailVerified)
            throw new VerificationCodeException("Email already Verified");
        
        await SendVerificationEmail((int)existingUser.Id, email, existingUser.FirstName);
    }

    public string GetOAuthUrl() => _oAuthService.GetAuthorizeUrl();

    public async Task<LoginResponse> LoginViaSocialMedia(string code)
    {
        var tokens = await _oAuthService.ExchangeCodeForTokenAsync(code);
        var userInfo = await _oAuthService.GetUserInfoAsync(tokens.AccessToken);
        if(userInfo == null)
            throw new UnauthorizedUserException();
        
        var user = await _userRepository.GetUserByEmailAsync(userInfo.Email);
        if (user == null)
            user = await RegisterUser(new CreateUserRequest()
            {
                Email = userInfo.Email,
                Role = UserRole.CUSTOMER,
                FirstName = userInfo.FullName.Split()[0],
                LastName = userInfo.FullName.Split()[1],
                Password = Guid.NewGuid().ToString()
            }, socialMediaRequest: true);

        if (user.Status == UserStatus.INACTIVE || !user.EmailVerified)
        {
            user.Status = UserStatus.ACTIVE;
            user.EmailVerified = true;
            user.UpdatedAt = DateTime.UtcNow;
            user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);
            await _userRepository.UpdateUserAsync(user);
        }

        return GetLoginResponse(user);
    }
    
    #region private methods
    
    private LoginResponse GetLoginResponse(User user)
    {
        return new LoginResponse()
        {
            AccessToken = JwtHelper.GenerateToken(user, _appSettings.JwtSettings, 30),
            RefreshToken = JwtHelper.GenerateToken(user, _appSettings.JwtSettings, 120, false),
            UserRole = user.Role
        };
    }

    private async Task SendVerificationEmail(int userId, string email, string customerName)
    {   
        var verificationCode = new VerificationCodes()
        {
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            VerificationCode = new Random().Next(1000000, 10000000).ToString(),
            IsUsed = false
        };

        await _emailService.SendEmailAsync(_appSettings.MailSettings, new EmailSendRequest()
        {
            IsBodyHtml = true,
            To = email,
            Subject = Constants.VerificationEmailSubject,
            Body = VerificationEmailTemplate.Generate(customerName, verificationCode.VerificationCode)
        });
        
        await _verificationCodeRepository.AddVerificationCode(verificationCode);
    }
    
    #endregion
}

using Application.CQRS.Command;
using Application.DTOs;
using Application.Interface;
using Application.Result;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.CQRS.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ServiceResult<LoginResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJWTService _jwtService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RegisterCommand> _validator;  
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        UserManager<AppUser> userManager,
        IJWTService jwtService,
        IUnitOfWork unitOfWork,
        IValidator<RegisterCommand> validator,        
        ILogger<RegisterCommandHandler> logger)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ServiceResult<LoginResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        //  VALIDATION password, email
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Registration validation failed for {Email}", request.Email);
            return ServiceResult<LoginResponse>.BadRequest(
                "Validation failed",
                validationResult.Errors.Select(e => e.ErrorMessage));
        }

        // Proceed with registration
        var user = new AppUser
        {
            FirstName = $"{request.FirstName} {request.LastName}",
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Registration failed for {Email}: {Errors}", request.Email, result.Errors);
            return ServiceResult<LoginResponse>.Failure(
                "Registration failed",
                400,
                result.Errors.Select(e => e.Description));
        }

        // Assign default role
        await _userManager.AddToRoleAsync(user, SystemRoles.User);

        // Generate tokens
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User registered successfully: {Email}", user.Email);

        return ServiceResult<LoginResponse>.Created(
            new LoginResponse(accessToken, DateTime.UtcNow.AddMinutes(60), refreshToken),
            "Registration successful");
    }
}
using Application.CQRS.Command;
using Application.DTOs;
using Application.Interface;
using Application.Result;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ServiceResult<LoginResponse>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJWTService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJWTService jwtService,
            IUnitOfWork unitOfWork,
            ILogger<LoginCommandHandler> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResult<LoginResponse>> Handle(LoginCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found: {Email}", request.Email);
                return ServiceResult<LoginResponse>.Failure("Invalid credentials", 401);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed for {Email} - invalid password", request.Email);
                return ServiceResult<LoginResponse>.Failure("Invalid credentials", 401);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var tokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _unitOfWork.RefreshTokens.AddAsync(tokenEntity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            return ServiceResult<LoginResponse>.Success(
                new LoginResponse(accessToken, DateTime.UtcNow.AddMinutes(60), refreshToken),
                "Login successful",
                200);
        }
    }
}

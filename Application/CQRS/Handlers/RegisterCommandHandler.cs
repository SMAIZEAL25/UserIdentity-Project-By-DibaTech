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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ServiceResult<LoginResponse>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJWTService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            UserManager<AppUser> userManager,
            IJWTService jwtService,
            IUnitOfWork unitOfWork,
            ILogger<RegisterCommandHandler> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResult<LoginResponse>> Handle(RegisterCommand request, CancellationToken ct)
        {
            var user = new AppUser
            {
                FirstName = $"{request.FirstName} {request.LastName}",
                Email = request.Email,
                UserName = request.Email,
                LastName = request.LastName
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Registration failed for {Email}", request.Email);
                return ServiceResult<LoginResponse>.Failure(
                    "Registration failed",
                    400,
                    result.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, SystemRoles.User);
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User registered: {Email}", user.Email);

            return ServiceResult<LoginResponse>.Created(
                new LoginResponse(accessToken, DateTime.UtcNow.AddMinutes(60), refreshToken),
                "Registration successful");
        }
    }
}

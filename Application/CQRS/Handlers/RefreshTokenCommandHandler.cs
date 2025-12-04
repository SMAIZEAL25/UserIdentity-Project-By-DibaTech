using Application.CQRS.Command;
using Application.DTOs;
using Application.Interface;
using Application.Result;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ServiceResult<LoginResponse>>
    {
        private readonly IJWTService _jwtService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenCommandHandler(
            IJWTService jwtService,
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(request.RefreshToken);
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return ServiceResult<LoginResponse>.Failure("Invalid token", 401);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return ServiceResult<LoginResponse>.Failure("Invalid token", 401);

            var savedToken = await _unitOfWork.RefreshTokens
                .GetPagedAsync(1, 1, rt => rt.Token == request.RefreshToken
                                        && rt.ExpiresAt > DateTime.UtcNow
                                        && !rt.IsRevoked);

            if (savedToken.Items.Count == 0)
                return ServiceResult<LoginResponse>.Failure("Invalid or expired refresh token", 401);

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Revoke old + issue new
            var oldToken = savedToken.Items.First();
            oldToken.IsRevoked = true;

            await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<LoginResponse>.Success(
                new LoginResponse(newAccessToken, DateTime.UtcNow.AddMinutes(60), newRefreshToken),
                "Token refreshed",
                200);
        }
    }
}

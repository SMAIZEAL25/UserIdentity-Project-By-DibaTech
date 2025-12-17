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
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Handlers
{
    public class Verify2FACommandHandler : IRequestHandler<Verify2FACommand, ServiceResult<LoginResponse>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITwoFactorService _twoFactorService;
        private readonly IJWTService _jwtService;
        private readonly IUnitOfWork _unitOfWork;

        public Verify2FACommandHandler(UserManager<AppUser> userManager, ITwoFactorService twoFactorService, IJWTService jwtService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _twoFactorService = twoFactorService;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<LoginResponse>> Handle(Verify2FACommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(request.email); 
            if (user == null || string.IsNullOrEmpty(user.TwoFactorSecret))
                return ServiceResult<LoginResponse>.Failure("2FA not set up", 400);

            if (!_twoFactorService.VerifyCode(user.TwoFactorSecret, request.Code))
                return ServiceResult<LoginResponse>.Failure("Invalid 2FA code", 401);

            // SUCCESS — Enable 2FA permanently
            user.TwoFactorEnabled = true;
            await _userManager.UpdateAsync(user);

            // Issue tokens
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken { UserId = user.Id, Token = refreshToken, ExpiresAt = DateTime.UtcNow.AddDays(7) });
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<LoginResponse>.Success(
                new LoginResponse(accessToken, DateTime.UtcNow.AddMinutes(60), refreshToken),
                "2FA verified - login successful");
        }
    }
}

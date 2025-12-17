using Application.CQRS.Command;
using Application.Interface;
using Application.Result;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Handlers
{
    [Authorize]
    public class Enable2FACommandHandler : IRequestHandler<Enable2FACommand, ServiceResult<Enable2FAResponse>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITwoFactorService _twoFactorService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Enable2FACommandHandler(
            UserManager<AppUser> userManager, 
            ITwoFactorService twoFactorService, 
            IHttpContextAccessor httpContextAccessor)

        {
            _userManager = userManager;
            _twoFactorService = twoFactorService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResult<Enable2FAResponse>> Handle(Enable2FACommand request, CancellationToken ct)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null) return ServiceResult<Enable2FAResponse>.Unauthorized();

            if (user.TwoFactorEnabled)
                return ServiceResult<Enable2FAResponse>.BadRequest("2FA already enabled");

            var (secret, qrCodeUrl, manualKey) = _twoFactorService.GenerateSecret(user.Email!);

            user.TwoFactorSecret = secret; // In real app: encrypt this!
            await _userManager.UpdateAsync(user);

            return ServiceResult<Enable2FAResponse>.Success(
                new Enable2FAResponse(qrCodeUrl, manualKey, secret),
                "Scan QR code with your authenticator app");
        }
    }
}

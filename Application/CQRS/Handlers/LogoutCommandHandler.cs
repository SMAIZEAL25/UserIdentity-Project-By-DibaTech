using Application.CQRS.Command;
using Application.Interface;
using Application.Result;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Handlers
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ServiceResult<object>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<LogoutCommandHandler> logger)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ServiceResult<object>> Handle(LogoutCommand request, CancellationToken ct)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return ServiceResult<object>.Unauthorized("User not authenticated");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return ServiceResult<object>.Unauthorized("User not found");

            // Revoke ALL refresh tokens for this user
            var allTokens = await _unitOfWork.RefreshTokens.GetAllAsync();
            var activeTokens = allTokens
                .Where(rt => rt.UserId == user.Id && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
                .ToList();

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User logged out and all tokens revoked: {UserId}", userId);

            return ServiceResult<object>.Success(
                data: new { message = "Logged out successfully" },
                message: "Logout successful",
                statusCode: 200);
        }
    }
}

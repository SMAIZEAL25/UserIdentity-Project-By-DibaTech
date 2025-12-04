//using Application.DTOs;
//using Application.Interface;
//using Application.Result;
//using AutoMapper;
//using Domain.Entities;
//using FluentValidation;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore.Metadata;
//using Microsoft.Extensions.Logging;
//using System.Security.Claims;


//namespace Application.Services
//{
//    public class AuthService : IAuthService
//    {
//        private readonly UserManager<AppUser> _userManager;
//        private readonly SignInManager<AppUser> _signInManager;
//        private readonly IJWTService _jwtService;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly ILogger<AuthService> _logger;

//        public AuthService(
//            UserManager<AppUser> userManager,
//            SignInManager<AppUser> signInManager,
//            IJWTService jwtService,
//            IUnitOfWork unitOfWork,
//            ILogger<AuthService> logger)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _jwtService = jwtService;
//            _unitOfWork = unitOfWork;
//            _logger = logger;
//        }

//        //public async Task<ServiceResult<LoginResponse>> RegisterAsync(RegisterRequest request)
//        //{
//        //    var user = new AppUser
//        //    {
//        //        FirstName = $"{request.FirstName} {request.LastName}",
//        //        Email = request.Email,
//        //        UserName = request.Email,
//        //        LastName = request.LastName
//        //    };

//        //    var result = await _userManager.CreateAsync(user, request.Password);

//        //    if (!result.Succeeded)
//        //    {
//        //        _logger.LogWarning("Registration failed for {Email}: {Errors}", request.Email, result.Errors);
//        //        return ServiceResult<LoginResponse>.Failure(
//        //            "Registration failed",
//        //            400,
//        //            result.Errors.Select(e => e.Description));
//        //    }

//        //    await _userManager.AddToRoleAsync(user, SystemRoles.User);

//        //    var roles = await _userManager.GetRolesAsync(user);
//        //    var accessToken = _jwtService.GenerateAccessToken(user, roles);
//        //    var refreshToken = _jwtService.GenerateRefreshToken();

//        //    var refreshTokenEntity = new RefreshToken
//        //    {
//        //        UserId = user.Id,
//        //        Token = refreshToken,
//        //        ExpiresAt = DateTime.UtcNow.AddDays(7),
//        //        JwtId = string.Empty // will be set on first use
//        //    };

//        //    await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenEntity);
//        //    await _unitOfWork.SaveChangesAsync();

//        //    _logger.LogInformation("User registered successfully: {Email}", user.Email);

//        //    return ServiceResult<LoginResponse>.Success(
//        //        new LoginResponse(accessToken, DateTime.UtcNow.AddMinutes(60), refreshToken),
//        //        "Registration successful",
//        //        201);
//        //}

//        public async Task<ServiceResult<LoginResponse>> LoginAsync(string email, string password)
//        {
//            var user = await _userManager.FindByEmailAsync(email);
//            if (user == null)
//            {
//                _logger.LogWarning("Login failed - user not found: {Email}", email);
//                return ServiceResult<LoginResponse>.Failure("Invalid credentials", 401);
//            }

//            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
//            if (!result.Succeeded)
//            {
//                _logger.LogWarning("Login failed for {Email} - invalid password", email);
//                return ServiceResult<LoginResponse>.Failure("Invalid credentials", 401);
//            }

//            var roles = await _userManager.GetRolesAsync(user);
//            var accessToken = _jwtService.GenerateAccessToken(user, roles);
//            var refreshToken = _jwtService.GenerateRefreshToken();

//            var tokenEntity = new RefreshToken
//            {
//                UserId = user.Id,
//                Token = refreshToken,
//                ExpiresAt = DateTime.UtcNow.AddDays(7)
//            };

//            await _unitOfWork.Repository<RefreshToken>().AddAsync(tokenEntity);
//            await _unitOfWork.SaveChangesAsync();

//            _logger.LogInformation("User logged in: {Email}", email);

//            return ServiceResult<LoginResponse>.Success(
//                new LoginResponse(accessToken, DateTime.UtcNow.AddMinutes(60), refreshToken),
//                "Login successful",
//                200);
//        }

//        public async Task<ServiceResult<LoginResponse>> RefreshTokenAsync(string token)
//        {
//            var principal = _jwtService.GetPrincipalFromExpiredToken(token);

//            var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var guidId))
//                return ServiceResult<LoginResponse>.Failure("Invalid token", 401);

//            var user = await _userManager.FindByIdAsync(guidId.ToString());
//            if (user == null)
//                return ServiceResult<LoginResponse>.Failure("Invalid token", 401);

//            var savedToken = await _unitOfWork.Repository<RefreshToken>()
//                .GetPagedAsync(1, 1, rt => rt.Token == token && rt.ExpiresAt > DateTime.UtcNow && !rt.IsRevoked);

//            if (savedToken.Items.Count == 0)
//                return ServiceResult<LoginResponse>.Failure("Invalid or expired refresh token", 401);

//            var roles = await _userManager.GetRolesAsync(user);
//            var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
//            var newRefreshToken = _jwtService.GenerateRefreshToken();

//            // Rotate refresh token
//            var oldToken = savedToken.Items.First();
//            oldToken.IsRevoked = true;

//            await _unitOfWork.Repository<RefreshToken>().AddAsync(new RefreshToken
//            {
//                UserId = user.Id,
//                Token = newRefreshToken,
//                ExpiresAt = DateTime.UtcNow.AddDays(7)
//            });

//            await _unitOfWork.SaveChangesAsync();

//            return ServiceResult<LoginResponse>.Success(
//                new LoginResponse(newAccessToken, DateTime.UtcNow.AddMinutes(60), newRefreshToken),
//                "Token refreshed",
//                200);
//        }
//    }
//}



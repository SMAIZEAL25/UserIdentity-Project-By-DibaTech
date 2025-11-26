using Application.DTOs;
using Application.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IAuthService
    {
        Task<ServiceResult<LoginResponse>> LoginAsync(string email, string password);
        Task<ServiceResult<LoginResponse>> RefreshTokenAsync(string token);
        Task<ServiceResult<LoginResponse>> RegisterAsync(RegisterRequest request);

        // Task<ServiceResult> RevokeRefreshTokenAsync(string token);
    }
}

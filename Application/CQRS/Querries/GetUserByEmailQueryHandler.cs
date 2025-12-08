
using Application.DTOs;
using Application.Result;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Querries
{
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, ServiceResult<UserDto>>
    {
        private readonly UserManager<AppUser> _userManager;

        public GetUserByEmailQueryHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ServiceResult<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return ServiceResult<UserDto>.Failure("User not found", 404);

            var roles = await _userManager.GetRolesAsync(user);

            var dto = new UserDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.UserName,
                roles.ToList(),
                user.CreatedAt
                );

            return ServiceResult<UserDto>.Success(dto, "User found", 200);
        }
    }
}

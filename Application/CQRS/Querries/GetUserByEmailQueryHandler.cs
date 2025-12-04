
using Application.DTOs;
using Application.Result;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public GetUserByEmailQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ServiceResult<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return ServiceResult<UserDto>.Failure("User not found", 404);

            var roles = await _userManager.GetRolesAsync(user);
            var dto = _mapper.Map<UserDto>(user) with { Roles = roles.ToList() };

            return ServiceResult<UserDto>.Success(dto, "User found", 200);
        }
    }
}

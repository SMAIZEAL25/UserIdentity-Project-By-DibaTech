using Application.CQRS.Querries;
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

namespace Application.CQRS.Command
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ServiceResult<UserDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ServiceResult<UserDto>> Handle(UpdateUserCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return ServiceResult<UserDto>.Failure("User not found", 404);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.UserName = request.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return ServiceResult<UserDto>.Failure("Update failed", 400, result.Errors.Select(e => e.Description));

            var roles = await _userManager.GetRolesAsync(user);
            var dto = _mapper.Map<UserDto>(user) with { Roles = roles.ToList() };

            return ServiceResult<UserDto>.Success(dto, "User updated successfully", 200);
        }
    }
}

using Application.CQRS.Command;
using Application.DTOs;
using Application.Result;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ServiceResult<UserDto>>
    {
        private readonly UserManager<AppUser> _userManager;
       
        private readonly IValidator<UpdateUserCommand> _validator;

        public UpdateUserCommandHandler(
        UserManager<AppUser> userManager,
   
        IValidator<UpdateUserCommand> validator)
        {
            _userManager = userManager;            
            _validator = validator;
        }


        public async Task<ServiceResult<UserDto>> Handle(UpdateUserCommand request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                return ServiceResult<UserDto>.Failure(
                    "Validation failed",
                    400,
                    validationResult.Errors.Select(e => e.ErrorMessage));
            }

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
            /*var dto = _mapper.Map<UserDto>(user) with { Roles = roles.ToList() };*/

            var dto = new UserDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.UserName,
                roles.ToList(),   
                user.CreatedAt    
                );

            return ServiceResult<UserDto>.Success(dto, "User updated successfully", 200);
        }
    }
}

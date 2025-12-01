using Application.Result;
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
    // Assign Role to User (Admin Only)
    public class AssignRoleCommand : IRequest<ServiceResult<string>>
    {
        public string UserEmail { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }

    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, ServiceResult<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AssignRoleCommandHandler(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ServiceResult<string>> Handle(AssignRoleCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(request.UserEmail);
            if (user == null) return ServiceResult<string>.Failure("User not found", 404);

            if (!await _roleManager.RoleExistsAsync(request.RoleName))
                return ServiceResult<string>.Failure("Role does not exist", 400);

            var result = await _userManager.AddToRoleAsync(user, request.RoleName);

            return result.Succeeded
                ? ServiceResult<string>.Success("Role assigned successfully", "Success", 200)
                : ServiceResult<string>.Failure("Failed to assign role", 400, result.Errors.Select(e => e.Description));
        }
    }
}

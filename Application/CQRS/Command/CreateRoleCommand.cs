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
    // Create Role (Admin Only)
    public class CreateRoleCommand : IRequest<ServiceResult<CreateRoleResponse>>
    {
        public string Name { get; set; } = null!;
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ServiceResult<CreateRoleResponse>>
    {
        private readonly RoleManager<AppRole> _roleManager;

        public CreateRoleCommandHandler(RoleManager<AppRole> roleManager)
            => _roleManager = roleManager;

        public async Task<ServiceResult<CreateRoleResponse>> Handle(CreateRoleCommand request, CancellationToken ct)
        {
            if (await _roleManager.RoleExistsAsync(request.Name))
                return ServiceResult<CreateRoleResponse>.Failure("Role already exists", 409);

            var role = new AppRole { Name = request.Name, NormalizedName = request.Name.ToUpper() };
            var result = await _roleManager.CreateAsync(role);

            return result.Succeeded
                ? ServiceResult<CreateRoleResponse>.Success(
                    new CreateRoleResponse(role.Name, "Role created"), "Role created", 201)
                : ServiceResult<CreateRoleResponse>.Failure("Failed to create role", 400, result.Errors.Select(e => e.Description));
        }
    }
}

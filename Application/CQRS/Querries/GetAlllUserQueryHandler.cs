using Application.DTOs;
using Application.Interface;
using Application.Result;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Querries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ServiceResult<PaginatedResult<UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        // Removed IMapper
        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<ServiceResult<PaginatedResult<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken ct)
        {
            // Build filter expression if search term is provided
            Expression<Func<AppUser, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                filter = u => u.Email!.Contains(request.Search) || u.FirstName.Contains(request.Search);
            }

            // Retrieve paginated users from repository
            var pagedUsers = await _unitOfWork.Users.GetPagedAsync(
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                filter: filter,
                orderBy: q => q.OrderBy(u => u.Email),
                includeProperties: "" // No navigation props needed
            );

            // Manual mapping
            var userDtos = new List<UserDto>();
            foreach (var user in pagedUsers.Items)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var dto = new UserDto(
                     user.Id,
                     user.FirstName,
                     user.LastName,
                     user.UserName,
                     roles.ToList(),
                     user.CreatedAt
                );
                userDtos.Add(dto);
            }

            // Build paginated result
            var result = new PaginatedResult<UserDto>(
                userDtos,
                pagedUsers.TotalCount,
                pagedUsers.PageNumber,
                pagedUsers.PageSize
            );

            return ServiceResult<PaginatedResult<UserDto>>.Success(result, "Users retrieved", 200);
        }
    }
}


using Application.DTOs;
using Application.Interface;
using Application.Result;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Querries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ServiceResult<PaginatedResult<UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ServiceResult<PaginatedResult<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken ct)
        {
            var filter = string.IsNullOrWhiteSpace(request.Search)
                ? (Expression<Func<AppUser, bool>>?)null
                : u => u.Email!.Contains(request.Search) || u.FirstName.Contains(request.Search);

            var pagedUsers = await _unitOfWork.Users.GetPagedAsync(
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                filter: filter,
                orderBy: q => q.OrderBy(u => u.Email),
                includeProperties: "" // No navigation props needed
            );

            var userDtos = new List<UserDto>();
            foreach (var user in pagedUsers.Items)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var dto = _mapper.Map<UserDto>(user);
                dto = dto with { Roles = roles.ToList() };
                userDtos.Add(dto);
            }

            var result = new PaginatedResult<UserDto>(
                userDtos,
                pagedUsers.TotalCount,
                pagedUsers.PageNumber,
                pagedUsers.PageSize);

            return ServiceResult<PaginatedResult<UserDto>>.Success(result, "Users retrieved", 200);
        }
    }
}

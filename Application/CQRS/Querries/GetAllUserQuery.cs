
using Application.DTOs;
using Application.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Querries
{
    public record GetAllUsersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null) : IRequest<ServiceResult<PaginatedResult<UserDto>>>;

    public record GetUserByEmailQuery(string Email) : IRequest<ServiceResult<UserDto>>;
}

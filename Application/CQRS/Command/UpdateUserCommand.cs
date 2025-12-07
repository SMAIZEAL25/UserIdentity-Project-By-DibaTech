using Application.DTOs;
using Application.Result;
using MediatR;

namespace Application.CQRS.Command
{
    public record UpdateUserCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Email) : IRequest<ServiceResult<UserDto>>;
}

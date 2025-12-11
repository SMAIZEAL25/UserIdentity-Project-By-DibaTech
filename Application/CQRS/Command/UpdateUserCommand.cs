using Application.DTOs;
using Application.Result;
using MediatR;

namespace Application.CQRS.Command
{
    public record UpdateUserCommand(

        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string Password,
        string ConfrimedPassword) : IRequest<ServiceResult<UserDto>>;
}

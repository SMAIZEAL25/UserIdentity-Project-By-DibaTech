using Application.DTOs;
using Application.Result;
using MediatR;

namespace Application.CQRS.Command
{
    public record RefreshTokenCommand(string RefreshToken)
    : IRequest<ServiceResult<LoginResponse>>;



};
    

    


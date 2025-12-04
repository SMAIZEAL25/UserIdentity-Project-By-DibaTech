using Application.DTOs;
using Application.Result;
using MediatR;

namespace Application.CQRS.Command
{
    
        public record LoginCommand (
            string Email,
            string Password
            
            ) : IRequest<ServiceResult<LoginResponse>>;
};
    

    


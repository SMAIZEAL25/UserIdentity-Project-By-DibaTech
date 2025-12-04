using Application.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Command
{
    public record LogoutCommand : IRequest<ServiceResult<object>>;
}

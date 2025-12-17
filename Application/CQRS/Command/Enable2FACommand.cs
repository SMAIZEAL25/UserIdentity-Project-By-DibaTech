using Application.DTOs;
using Application.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Command
{
    public record Enable2FACommand : IRequest<ServiceResult<Enable2FAResponse>>;
   
    public record Enable2FAResponse (string QRCodeImageUrl, string ManualEntryKey, string Seceretkey);

    public record Verify2FACommand (string email, string Code) : IRequest<ServiceResult<LoginResponse>>;

    public record LoginWith2FAcommand (string email, string Code, string? RememberMe = null ) : IRequest<ServiceResult<LoginResponse>>;
}

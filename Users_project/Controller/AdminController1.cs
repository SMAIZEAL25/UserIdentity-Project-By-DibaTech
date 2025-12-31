using Application.CQRS.Command;
using Application.CQRS.Querries;
using Application.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Reflection;

namespace Users_project.Controller
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Policy = "RequireAdmin")]
    [EnableRateLimiting("UserBasedRateLimit")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) => _mediator = mediator;

        // GET: api/admin/users?pageNumber=2&pageSize=10&search=john
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        // GET: api/admin/users/email/john@example.com
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var result = await _mediator.Send(new GetUserByEmailQuery(email));
            return result.ToActionResult();
        }

        // PUT: api/admin/users/emailAddress
        [HttpPut("{Email}")]
        public async Task<IActionResult> Update(string email, [FromBody] UpdateUserCommand command)
        {
            command = command with { Email = email };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        // Use Soft Delete for future audit purpose


    }
}




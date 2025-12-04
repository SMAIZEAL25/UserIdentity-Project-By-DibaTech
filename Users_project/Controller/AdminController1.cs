using Application.CQRS.Querries;
using Application.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Users_project.Controller
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Policy = "RequireAdmin")]
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

        // PUT: api/admin/users/123e4567-e89b-12d3-a456-426614174000
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            command = command with { Id = id };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }
    }
}




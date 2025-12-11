using Application.CQRS.Command;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Application.Result;

namespace Users_project.Controller
{
    [ApiController]
    [Route("api/RequireManager")]
    [Authorize(Policy = "RequireManagerOrAdmin")]
    public class ManagerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;    // For Delete i will need to use soft delete for future audit purpose using Bool data type 

        public ManagerController(
            IMediator mediator,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _mediator = mediator;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [HttpPost("Create-Roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var command = new CreateRoleCommand { Name = request.Name };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }



        [HttpPost("assign-Role")]
       
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var command = new AssignRoleCommand
            {
                UserEmail = request.UserEmail,
                RoleName = request.RoleName
            };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }



        [HttpPost("remove-Role")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.UserEmail);
            if (user == null) return NotFound();

            var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
            return result.Succeeded ? Ok("Role removed") : BadRequest(result.Errors);
        }



        [HttpDelete("Delete-Roles/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return NotFound();

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded ? Ok("Role deleted") : BadRequest(result.Errors); 
        }

        

    }

}

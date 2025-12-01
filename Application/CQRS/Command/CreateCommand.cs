using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Command
{
    public record CreateRoleRequest(string Name);
    public record CreateRoleResponse(string Name, string Message);

    public record AssignRoleRequest(string UserEmail, string RoleName);
    public record RemoveRoleRequest(string UserEmail, string RoleName);
}

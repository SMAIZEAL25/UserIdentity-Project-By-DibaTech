namespace Application.CQRS.Command
{
    public record RemoveRoleRequest(string UserEmail, string RoleName);
}

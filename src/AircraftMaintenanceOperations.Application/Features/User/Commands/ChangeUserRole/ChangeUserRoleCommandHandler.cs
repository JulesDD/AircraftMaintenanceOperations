namespace AircraftMaintenanceOperations.Application.Features.User.Commands.ChangeUserRole;

public class ChangeUserRoleCommandHandler(IAircraftMaintenanceDbContext DbContext, IIdentityRoleService IdentityRoleService) : ICommandHandler<ChangeUserRoleCommand, ChangeUserRoleCommandResult>
{
    public async Task<ChangeUserRoleCommandResult> Handle(ChangeUserRoleCommand command, CancellationToken cancellationToken)
    {
        var user = await DbContext.Users.FindAsync([command.UserId], cancellationToken);
        if (user == null) throw new InvalidOperationException("User not found.");

        var previousRole = user.Role;
        user.ChangeRole(command.NewRole);

        await IdentityRoleService.UpdateUserRoleAsync(user.Id, command.NewRole, cancellationToken);

        await DbContext.SaveChangesAsync(cancellationToken);
        return new ChangeUserRoleCommandResult(user.Id, previousRole, command.NewRole);
    }
}

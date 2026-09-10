namespace AircraftMaintenanceOperations.Application.Features.User.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(Guid UserId, Role NewRole) : ICommand<ChangeUserRoleCommandResult>;
public record ChangeUserRoleCommandResult(Guid UserId, Role PreviousRole, Role NewRole);

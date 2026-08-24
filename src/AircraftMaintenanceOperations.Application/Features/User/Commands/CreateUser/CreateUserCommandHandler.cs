namespace AircraftMaintenanceOperations.Application.Features.User.Commands.CreateUser;

public class CreateUserCommandHandler(IAircraftMaintenanceDbContext DbContext) : ICommandHandler<CreateUserCommand, CreateUserCommandResult>
{
    public async Task<CreateUserCommandResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = Domain.Entities.User.Create
        (
            command.EmployeeNumber,
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.Role
        );

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync(cancellationToken);
        return new CreateUserCommandResult(user.Id);
    }
}

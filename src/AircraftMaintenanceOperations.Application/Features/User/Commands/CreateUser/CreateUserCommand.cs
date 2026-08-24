namespace AircraftMaintenanceOperations.Application.Features.User.Commands.CreateUser;

public record CreateUserCommand(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    Role Role
) : ICommand<CreateUserCommandResult>;

public record CreateUserCommandResult(Guid Id);

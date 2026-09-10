namespace AircraftMaintenanceOperations.Application.Features.User.Commands.ChangeUserRole;

public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.NewRole).IsInEnum().WithMessage("NewRole must be a valid role.");
    }
}

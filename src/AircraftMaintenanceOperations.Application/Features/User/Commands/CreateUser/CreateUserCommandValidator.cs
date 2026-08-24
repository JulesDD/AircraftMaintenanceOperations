namespace AircraftMaintenanceOperations.Application.Features.User.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("User must have a first name.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("User must have a last name.");
        RuleFor(x => x.Email).EmailAddress().NotEmpty().WithMessage("A valid email address is required.");
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+?1?\s*\(?[2-9]\d{2}\)?[-.\s]?\d{3}[-.\s]?\d{4}$").WithMessage("A valid North American phone number is required.");
        RuleFor(x => x.EmployeeNumber).NotEmpty().WithMessage("Employee number is required");
        RuleFor(x => x.Role).IsInEnum().WithMessage("A valid role is required.");
    }
}

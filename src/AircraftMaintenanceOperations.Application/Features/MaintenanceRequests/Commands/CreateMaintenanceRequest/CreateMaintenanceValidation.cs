namespace AircraftMaintenanceOperations.Application.Features.MaintenanceRequests.Commands.CreateMaintenanceRequest;

public class CreateMaintenanceValidation : AbstractValidator<CreateMaintenanceCommand>
{
    public CreateMaintenanceValidation() 
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Please provide a title for maintenance request.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Please provide some description in the request.");
        RuleFor(x => x.AircraftId).NotEmpty().WithMessage("Please provide an aircraft ID number");
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("The due date must be a day ahead from the date it was created.");
        
    }
}

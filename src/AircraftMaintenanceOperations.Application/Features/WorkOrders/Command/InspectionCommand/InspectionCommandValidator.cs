namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.InspectionCommand;

public class InspectionCommandValidator : AbstractValidator<InspectionCommand>
{
    public InspectionCommandValidator()
    {
        RuleFor(ip => ip.WorkOrderId).NotEmpty().WithMessage("Work Order ID is required.");
        RuleFor(ip => ip.LaborNotes).NotEmpty().WithMessage("Labor Notes are required.");
        RuleFor(ip => ip.LaborNotes).MaximumLength(500).WithMessage("Labor Notes cannot exceed 500 characters.");
    }
}

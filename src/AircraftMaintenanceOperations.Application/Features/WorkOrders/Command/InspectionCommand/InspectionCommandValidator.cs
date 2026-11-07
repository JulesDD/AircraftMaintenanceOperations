namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.InspectionCommand;

public class InspectionCommandValidator : AbstractValidator<InspectionCommand>
{
    public InspectionCommandValidator()
    {
        RuleFor(ip => ip.WorkOrderId).NotEmpty().WithMessage("Work Order ID is required.");
        RuleFor(ip => ip.InspectionNotes).NotEmpty().WithMessage("Inspection Notes are required.");
        RuleFor(ip => ip.InspectionNotes).MaximumLength(500).WithMessage("Inspection Notes cannot exceed 500 characters.");
    }
}

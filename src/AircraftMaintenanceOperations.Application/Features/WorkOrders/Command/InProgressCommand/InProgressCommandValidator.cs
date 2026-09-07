namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.InProgressCommand;

public class InProgressCommandValidator : AbstractValidator<InProgressCommand>
{
    public InProgressCommandValidator()
    {
        RuleFor(ip => ip.WorkOrderId).NotEmpty().WithMessage("Work Order ID is required.");
        RuleFor(ip => ip.TechnicianId).NotEmpty().WithMessage("Technician ID is required.");
        RuleFor(ip => ip.LaborNotes).NotEmpty().WithMessage("Labor Notes are required.");
        RuleFor(ip => ip.LaborNotes).MaximumLength(500).WithMessage("Labor Notes cannot exceed 500 characters.");
    }
}

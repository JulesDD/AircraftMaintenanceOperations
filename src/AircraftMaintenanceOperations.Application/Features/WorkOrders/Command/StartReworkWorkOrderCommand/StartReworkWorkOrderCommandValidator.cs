namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.ReworkWorkOrderCommand;

public class StartReworkWorkOrderCommandValidator : AbstractValidator<StartReworkWorkOrderCommand>
{
    public StartReworkWorkOrderCommandValidator()
    {
        RuleFor(rw => rw.WorkOrderId).NotEmpty().WithMessage("Work Order ID is required.");
        RuleFor(rw => rw.ReworkNotes).Must(notes => !string.IsNullOrWhiteSpace(notes)).WithMessage("Rework notes are required.").MaximumLength(500).WithMessage("Rework notes cannot exceed 500 characters.");
    }
}

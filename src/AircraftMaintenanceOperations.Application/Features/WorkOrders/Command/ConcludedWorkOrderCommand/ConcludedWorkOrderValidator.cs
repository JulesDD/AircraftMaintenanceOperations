namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.ConcludedWorkOrderCommand;

public class ConcludedWorkOrderValidator : AbstractValidator<ConcludedWorkOrderCommand>
{
    public ConcludedWorkOrderValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty().WithMessage("Please provide a work order ID.");
        RuleFor(x => x.LaborNotes).NotEmpty().WithMessage("Labor notes are required.");
    }
}

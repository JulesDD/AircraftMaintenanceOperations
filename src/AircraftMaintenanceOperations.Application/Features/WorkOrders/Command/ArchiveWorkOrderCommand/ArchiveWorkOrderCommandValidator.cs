namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.ArchiveWorkOrderCommand;

public class ArchiveWorkOrderCommandValidator : AbstractValidator<ArchiveWorkOrderCommand>
{
    public ArchiveWorkOrderCommandValidator()
    {
        RuleFor(awo => awo.WorkOrderId).NotEmpty().WithMessage("Work Order ID is required.");
    }
}

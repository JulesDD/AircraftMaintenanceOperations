namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.OnHoldWorkOrderCommand;

public class OnHoldWorkOrderCommandValidator : AbstractValidator<OnHoldWorkOrderCommand>
{
    public OnHoldWorkOrderCommandValidator()
    {
        RuleFor(oh => oh.WorkOrderId).NotEmpty().WithMessage("Work Order ID is required.");
        RuleFor(oh => oh.HoldReason).IsInEnum().WithMessage("Hold Reason is required.");
        RuleFor(oh => oh.HoldNotes).NotEmpty().Must(notes => !string.IsNullOrWhiteSpace(notes)).WithMessage("Hold notes are required.");
        RuleFor(oh => oh.HoldNotes).MaximumLength(500).WithMessage("Hold Notes cannot exceed 500 characters.");
    }
}

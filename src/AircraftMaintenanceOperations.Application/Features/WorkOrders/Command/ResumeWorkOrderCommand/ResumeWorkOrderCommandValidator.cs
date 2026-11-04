namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.ResumeWorkOrderCommand;

public class ResumeWorkOrderCommandValidator : AbstractValidator<ResumeWorkOrderCommand>
{
    public ResumeWorkOrderCommandValidator()
    {
        RuleFor(r => r.WorkOrderId).NotEmpty().WithMessage("Work Order ID is required.");
        RuleFor(r => r.ResumeNotes).NotEmpty().Must(notes => !string.IsNullOrWhiteSpace(notes)).WithMessage("Resume notes are required.");
        RuleFor(r => r.ResumeNotes).MaximumLength(500).WithMessage("Resume Notes cannot exceed 500 characters.");
    }
}

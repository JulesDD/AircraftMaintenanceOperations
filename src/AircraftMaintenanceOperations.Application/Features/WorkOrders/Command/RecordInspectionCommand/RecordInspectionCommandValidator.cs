namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.RecordInspectionCommand;

public class RecordInspectionCommandValidator : AbstractValidator<RecordInspectionCommand>
{
    public RecordInspectionCommandValidator()
    {
        RuleFor(ip => ip.WorkOrderId).NotEmpty().WithMessage("Work Order ID is required.");
        RuleFor(x => x.InspectionNotes).Must(notes => !string.IsNullOrWhiteSpace(notes)).WithMessage("Inspection notes cannot be empty or whitespace.");
        RuleFor(ip => ip.InspectionNotes).NotEmpty().WithMessage("Inspection Notes are required.");
        RuleFor(ip => ip.InspectionNotes).MaximumLength(500).WithMessage("Inspection Notes cannot exceed 500 characters.");
    }
}

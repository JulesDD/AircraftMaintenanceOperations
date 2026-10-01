namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.CreateWorkOrderCommand;

public class CreateWorkOrderValidation : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderValidation()
    {
        RuleFor(x => x.MaintenanceRequestId).NotEmpty().WithMessage("Please provide a maintenance request ID.");
        RuleFor(x => x.AircraftId).NotEmpty().WithMessage("Please provide an aircraft ID.");
        RuleFor(x => x.WorkOrderPriority).IsInEnum().WithMessage("Please provide a valid work order priority.");
        RuleFor(x => x.EstimatedCompletionDate).Must(date => date >= DateTime.UtcNow).WithMessage("Estimated completion date cannot be in the past.");
        RuleFor(x => x.LaborNotes).NotEmpty().WithMessage("Work Order notes are required.");
    }
}

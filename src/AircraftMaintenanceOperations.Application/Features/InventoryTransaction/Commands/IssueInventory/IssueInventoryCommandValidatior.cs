namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.IssueInventory;

public class IssueIventoryCommandValidation : AbstractValidator<IssueInventoryCommand>
{
    public IssueIventoryCommandValidation()
    {
        RuleFor(x => x.InventoryPartId).NotEmpty().WithMessage("Please provide Inventory part Id");
        RuleFor(x => x.WorkOrderId).NotEmpty().WithMessage("Please provide Work Order Id");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero");
        RuleFor(x => x.Reason).MaximumLength(500).When(x => x.Reason is not null).WithMessage("Reason cannot be greater than 500 characters.");
    }
}

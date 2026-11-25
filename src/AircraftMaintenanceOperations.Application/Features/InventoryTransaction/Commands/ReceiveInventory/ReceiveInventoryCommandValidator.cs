namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.ReceiveInventory;

public class ReceiveInventoryCommandValidator : AbstractValidator<ReceiveInventoryCommand>
{
    public ReceiveInventoryCommandValidator()
    {
        RuleFor(ri => ri.InventoryPartId).NotEmpty().WithMessage("Please provide Inventory part Id.");
        RuleFor(ri => ri.InventoryLocationId).NotEmpty().WithMessage("Please provide Inventory location Id.");
        RuleFor(ri => ri.Quantity).GreaterThan(0).WithMessage("Provide a value greater than zero.");
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Please provide details for the inventory receipt.").MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
    }
}

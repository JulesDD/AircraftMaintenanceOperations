namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.ReceiveInventory;

public class ReceiveInventoryCommandValidator : AbstractValidator<ReceiveInventoryCommand>
{
    public ReceiveInventoryCommandValidator()
    {
        RuleFor(x => x.InventoryPartId).NotEmpty().WithMessage("Please provide Inventory part Id.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Provide a value greater than zero.");
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Provide details for item recieved.");
    }
}

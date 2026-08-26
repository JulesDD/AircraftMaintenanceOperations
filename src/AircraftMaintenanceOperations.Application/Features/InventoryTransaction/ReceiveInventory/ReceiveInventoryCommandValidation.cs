namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.ReceiveInventory;

public class ReceiveInventoryCommandValidation : AbstractValidator<ReceiveInventoryCommand>
{
    public ReceiveInventoryCommandValidation()
    {
        RuleFor(x => x.InventoryPartId).NotEmpty().WithMessage("Please provide Inventory part Id.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Provide a value greater than zero.");
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Provide details for item recieved.");
    }
}

namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.CreateInventory;

public class CreateInventoryCommandValidator : AbstractValidator<CreateInventoryCommand>
{
    public CreateInventoryCommandValidator()
    {
        RuleFor(x => x.PartNumber).NotEmpty().WithMessage("Part Number is required");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid Inventory Part Type");
        RuleFor(x => x.QuantityOnHand).GreaterThanOrEqualTo(0).WithMessage("Quantity On Hand must be a positive number");
        RuleFor(x => x.MinimumQuantity).GreaterThanOrEqualTo(0).WithMessage("Minimum Quantity must be a positive number");
        RuleFor(x => x.Location).NotEmpty().WithMessage("Location is required");
    }
}

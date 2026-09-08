namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.AdjustInventory;

public class AdjustInventoryCommandValidator : AbstractValidator<AdjustInventoryCommand>
{
    public AdjustInventoryCommandValidator()
    {
        RuleFor(x => x.InventoryPartId).NotEmpty().WithMessage("InventoryPartId is required.");
        RuleFor(x => x.Quantity).NotEqual(0).WithMessage("Adjustment quantity cannot be zero.");
        RuleFor(x => x.Notes).NotEmpty().WithMessage("Notes are required.");
        RuleFor(x => x.Notes).MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
    }
}

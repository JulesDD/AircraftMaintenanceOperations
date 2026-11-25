
namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.CreateInventory;

public class CreateInventoryCommandValidator : AbstractValidator<CreateInventoryCommand>
{
    public CreateInventoryCommandValidator()
    {
        RuleFor(x => x.PartNumber).NotEmpty().WithMessage("Part Number is required.").MaximumLength(50).WithMessage("Part Number cannot exceed 50 characters.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.").MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid Inventory Part Type.");
        RuleFor(x => x.MinimumQuantity).GreaterThanOrEqualTo(0).WithMessage("Minimum Quantity cannot be negative.");
    }
}

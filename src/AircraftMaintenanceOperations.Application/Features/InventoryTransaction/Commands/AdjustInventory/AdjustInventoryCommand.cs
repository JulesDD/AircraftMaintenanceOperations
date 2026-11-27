namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.AdjustInventory;

public record AdjustInventoryCommand(
    Guid InventoryPartId,
    int AdjustmentQuantity,
    Guid InventoryLocationId,
    InventoryCondition Condition,
    string? Notes) : ICommand<AdjustInventoryCommandResult>;
public record AdjustInventoryCommandResult(Guid Id);
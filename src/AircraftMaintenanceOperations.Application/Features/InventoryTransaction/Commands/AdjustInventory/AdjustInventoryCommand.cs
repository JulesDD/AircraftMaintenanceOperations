namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.AdjustInventory;

public record AdjustInventoryCommand(
    Guid InventoryPartId,
    int Quantity, 
    string? Notes) : ICommand<AdjustInventoryCommandResult>;
public record AdjustInventoryCommandResult(Guid Id);
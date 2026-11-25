namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.ReceiveInventory;

public record ReceiveInventoryCommand(
    Guid InventoryPartId,
    Guid InventoryLocationId,
    int Quantity, 
    string? Reason) : ICommand<ReceiveInventoryCommandResult>;

public record ReceiveInventoryCommandResult(Guid Id);
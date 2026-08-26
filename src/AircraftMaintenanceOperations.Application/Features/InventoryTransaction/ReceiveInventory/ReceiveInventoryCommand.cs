namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.ReceiveInventory;

public record ReceiveInventoryCommand
(
    Guid InventoryPartId,
    int Quantity,
    string? Reason) : ICommand<ReceiveInventoryCommandResult>;

public record ReceiveInventoryCommandResult(Guid Id);
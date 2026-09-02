namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.CreateInventory;

public record CreateInventoryCommand
(
    string PartNumber,
    string Description,
    InventoryPartType Type,
    int QuantityOnHand,
    int MinimumQuantity,
    string Location
) : ICommand<CreateInventoryCommandResult>;

public record CreateInventoryCommandResult(Guid Id);

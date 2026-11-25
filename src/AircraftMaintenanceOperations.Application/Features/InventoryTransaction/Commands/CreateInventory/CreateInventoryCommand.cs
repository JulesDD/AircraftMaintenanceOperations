namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.CreateInventory;

public record CreateInventoryCommand
(
    string PartNumber,
    string Description,
    InventoryPartType Type,
    int MinimumQuantity
) : ICommand<CreateInventoryCommandResult>;

public record CreateInventoryCommandResult(Guid Id);

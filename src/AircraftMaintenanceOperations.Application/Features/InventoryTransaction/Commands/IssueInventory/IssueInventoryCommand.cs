namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.IssueInventory;

public record IssueInventoryCommand(
    Guid InventoryPartId,
    Guid WorkOrderId,
    int Quantity,
    string? Reason) : ICommand<IssueInventoryCommandResult>;
public record IssueInventoryCommandResult(Guid Id);

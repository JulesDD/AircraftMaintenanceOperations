namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.AdjustInventory;

public class AdjustInventoryCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser) : ICommandHandler<AdjustInventoryCommand, AdjustInventoryCommandResult>
{
    public async Task<AdjustInventoryCommandResult> Handle(AdjustInventoryCommand command, CancellationToken cancellationToken)
    {
        var inventoryPart = await DbContext.InventoryParts.FindAsync(new object[] { command.InventoryPartId }, cancellationToken);
        if (inventoryPart is null) throw new InvalidOperationException($"Inventory part with ID {command.InventoryPartId} not found.");
        
        var quantityBefore = inventoryPart.QuantityOnHand;
        var result = inventoryPart.AdjustStock(command.Quantity);
        var quantityAfter = inventoryPart.QuantityOnHand;

        var inventoryTransaction = Domain.Entities.InventoryTransaction.Create(
            inventoryPart.Id,
            InventoryTransactionType.Adjustment,
            Math.Abs(command.Quantity),
            quantityBefore,
            quantityAfter,
            CurrentUser.DomainUserId,
            workOrderId: null,
            reason: command.Notes);

        DbContext.InventoryTransactions.Add(inventoryTransaction);
        await DbContext.SaveChangesAsync(cancellationToken);
        return new AdjustInventoryCommandResult(inventoryTransaction.Id);

    }
}

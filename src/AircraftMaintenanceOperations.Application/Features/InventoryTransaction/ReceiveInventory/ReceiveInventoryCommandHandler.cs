namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.ReceiveInventory;

internal class ReceiveInventoryCommandHandler(IAircraftMaintenanceDbContext dbContext, ICurrentUserService currentUser) : ICommandHandler<ReceiveInventoryCommand, ReceiveInventoryCommandResult>
{
    public async Task<ReceiveInventoryCommandResult> Handle(ReceiveInventoryCommand command, CancellationToken cancellationToken)
    {
        var inventoryPart = await dbContext.InventoryParts.FirstOrDefaultAsync(ip => ip.Id == command.InventoryPartId, cancellationToken);
        if (inventoryPart is null) throw new InvalidOperationException("Inventory part could not be found.");

        var quantityBefore = inventoryPart.QuantityOnHand;

        inventoryPart.ReceivedStock(command.Quantity);

        var quantityAfter = inventoryPart.QuantityOnHand;

        var transaction = Domain.Entities.InventoryTransaction.Create(
            inventoryPart.Id,
            InventoryTransactionType.Receipt,
            command.Quantity,
            quantityBefore,
            quantityAfter,
            currentUser.DomainUserId,
            reason: command.Reason);

        dbContext.InventoryTransactions.Add(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReceiveInventoryCommandResult(transaction.Id);
    }
}
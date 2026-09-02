namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.ReceiveInventory;

internal class ReceiveInventoryCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser) : ICommandHandler<ReceiveInventoryCommand, ReceiveInventoryCommandResult>
{
    public async Task<ReceiveInventoryCommandResult> Handle(ReceiveInventoryCommand command, CancellationToken cancellationToken)
    {
        var inventoryPart = await DbContext.InventoryParts.FirstOrDefaultAsync(ip => ip.Id == command.InventoryPartId, cancellationToken);
        if (inventoryPart is null) throw new InvalidOperationException("Inventory part could not be found.");

        var quantityBefore = inventoryPart.QuantityOnHand;

        inventoryPart.ReceivedStock(command.Quantity);

        var quantityAfter = inventoryPart.QuantityOnHand;

        var inventoryTransaction = Domain.Entities.InventoryTransaction.Create(
            inventoryPart.Id,
            InventoryTransactionType.Receipt,
            command.Quantity,
            quantityBefore,
            quantityAfter,
            CurrentUser.DomainUserId,
            reason: command.Reason);

        DbContext.InventoryTransactions.Add(inventoryTransaction);
        await DbContext.SaveChangesAsync(cancellationToken);
        return new ReceiveInventoryCommandResult(inventoryTransaction.Id);
    }
}
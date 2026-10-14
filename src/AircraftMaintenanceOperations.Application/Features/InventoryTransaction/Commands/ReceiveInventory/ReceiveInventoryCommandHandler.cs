namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.ReceiveInventory;

public class ReceiveInventoryCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser, IEventPublisher EventPublisher) : ICommandHandler<ReceiveInventoryCommand, ReceiveInventoryCommandResult>
{
    public async Task<ReceiveInventoryCommandResult> Handle(ReceiveInventoryCommand command, CancellationToken cancellationToken)
    {
        var inventoryPart = await DbContext.InventoryParts.FirstOrDefaultAsync(ip => ip.Id == command.InventoryPartId, cancellationToken);
        if (inventoryPart is null) throw new InvalidOperationException("Inventory part could not be found.");

        var quantityBefore = inventoryPart.QuantityOnHand;

        var result = inventoryPart.ReceivedStock(command.Quantity);
        if (!result.IsSuccess) throw new InvalidOperationException(result.ErrorMessage);

        var quantityAfter = inventoryPart.QuantityOnHand;

        var domainUserId = await CurrentUser.GetDomainUserIdAsync(cancellationToken);

        var inventoryTransaction = Domain.Entities.InventoryTransaction.Create(
            inventoryPart.Id,
            InventoryTransactionType.Receipt,
            command.Quantity,
            quantityBefore,
            quantityAfter,
            domainUserId,
            reason: command.Reason);

        DbContext.InventoryTransactions.Add(inventoryTransaction);
        await DbContext.SaveChangesAsync(cancellationToken);

        var inventoryTransactionCreatedEvent = new InventoryReceivedEvent
        {
            EventId = Guid.NewGuid(),
            ReceivedByUserId = domainUserId,
            InventoryPartId = inventoryTransaction.InventoryPartId,
            Quantity = command.Quantity,
            Reason = command.Reason,
            OccurredAt = DateTimeOffset.UtcNow
        };

        await EventPublisher.PublishAsync(inventoryTransactionCreatedEvent, cancellationToken);
        return new ReceiveInventoryCommandResult(inventoryTransaction.Id);
    }
}
namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.IssueInventory;

internal class IssueInventoryCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser) 
    : ICommandHandler<IssueInventoryCommand, IssueInventoryCommandResult>
{
    public async Task<IssueInventoryCommandResult> Handle(IssueInventoryCommand command, CancellationToken cancellationToken)
    {
        var inventory = await DbContext.InventoryParts.FirstOrDefaultAsync(i => i.Id == command.InventoryPartId, cancellationToken);
        if (inventory is null) throw new InvalidOperationException("Inventory could not be found");
        
        var workOrder = await DbContext.WorkOrders.FirstOrDefaultAsync(wo => wo.Id == command.WorkOrderId, cancellationToken);
        if (workOrder is null) throw new InvalidOperationException("Work order could not be found.");
        if (workOrder.WorkOrderStatus != WorkOrderStatus.InProgress) throw new InvalidOperationException("Inventory can only be issued to work orders that are in progress.");

        var quantityBefore = inventory.QuantityOnHand;

        inventory.Consume(command.Quantity);

        var quantityAfter = inventory.QuantityOnHand;

        var inventoryUsage = new InventoryUsage
        {
            InventoryPartId = inventory.Id,
            WorkOrderId = workOrder.Id,
            QuantityUsed = command.Quantity,
            DateUsed = DateTime.UtcNow
        };

        var inventoryTransaction = Domain.Entities.InventoryTransaction.Create(
        
            inventory.Id,
            InventoryTransactionType.Usage,
            command.Quantity,
            quantityBefore,
            quantityAfter,
            CurrentUser.DomainUserId,
            workOrderId: workOrder.Id,
            reason: command.Reason);
        
        DbContext.InventoryUsages.Add(inventoryUsage);
        DbContext.InventoryTransactions.Add(inventoryTransaction);

        try
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Failed to save inventory issue: {ex.InnerException?.Message ?? ex.Message}", ex);
        }

        return new IssueInventoryCommandResult(inventoryTransaction.Id);
    }
}

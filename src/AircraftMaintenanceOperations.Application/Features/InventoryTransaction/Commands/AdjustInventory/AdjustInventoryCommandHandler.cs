namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.AdjustInventory;


public class AdjustInventoryCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser) : ICommandHandler<AdjustInventoryCommand, AdjustInventoryCommandResult>
{
    public async Task<AdjustInventoryCommandResult> Handle(AdjustInventoryCommand command, CancellationToken cancellationToken)
    {
        var isAuthorized = CurrentUser.Roles.Any(role => string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) || string.Equals(role, "InventoryClerk", StringComparison.OrdinalIgnoreCase));
        if (!isAuthorized) throw new UnauthorizedAccessException("You are not authorized to adjust inventory.");

        var domainUserId = await CurrentUser.GetDomainUserIdAsync(cancellationToken);

        var inventoryStock = await DbContext.InventoryStocks.FirstOrDefaultAsync(s => s.InventoryPartId == command.InventoryPartId &&
                s.InventoryLocationId == command.InventoryLocationId && s.Condition == command.Condition, cancellationToken);
        if (inventoryStock is null) throw new InvalidOperationException("No stock balance was found for the specified part, location, and condition.");

        var inventoryLocation = await DbContext.InventoryLocations.FirstOrDefaultAsync(l => l.Id == command.InventoryLocationId, cancellationToken);
        if (inventoryLocation is null) throw new InvalidOperationException("Inventory location could not be found.");
        if (!inventoryLocation.IsActive) throw new InvalidOperationException("Cannot adjust inventory in an inactive location.");

        var quantityBefore = inventoryStock.Quantity;

        var adjustmentQuantity = Math.Abs(command.AdjustmentQuantity);

        var result = command.AdjustmentQuantity > 0 ? inventoryStock.IncreaseQuantity(adjustmentQuantity) : inventoryStock.DecreaseQuantity(adjustmentQuantity);

        if (!result.IsSuccess) throw new InvalidOperationException(result.ErrorMessage);

        var quantityAfter = inventoryStock.Quantity;

        var transaction = Domain.Entities.InventoryTransaction.Create(
            inventoryStock.InventoryPartId,
            InventoryTransactionType.Adjustment,
            inventoryStock.Condition,
            adjustmentQuantity,
            quantityBefore,
            quantityAfter,
            inventoryStock.InventoryLocationId,
            workOrderId: null,
            performedByUserId: domainUserId,
            transactionDate: DateTime.UtcNow,
            reason: command.Notes);

        DbContext.InventoryTransactions.Add(transaction);
        await DbContext.SaveChangesAsync(cancellationToken);
        return new AdjustInventoryCommandResult(transaction.Id);
    }
}

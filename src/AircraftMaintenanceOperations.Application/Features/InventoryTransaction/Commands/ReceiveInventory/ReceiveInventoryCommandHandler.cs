
namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.ReceiveInventory;

public class ReceiveInventoryCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser)
    : ICommandHandler<ReceiveInventoryCommand, ReceiveInventoryCommandResult>
{
    public async Task<ReceiveInventoryCommandResult> Handle(ReceiveInventoryCommand command, CancellationToken cancellationToken)
    {
        var isAuthorized = CurrentUser.Roles.Any(role => string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, "InventoryClerk", StringComparison.OrdinalIgnoreCase));
        if (!isAuthorized) throw new UnauthorizedAccessException("You are not authorized to receive inventory.");

        var domainUserId = await CurrentUser.GetDomainUserIdAsync(cancellationToken);

        var inventoryPart = await DbContext.InventoryParts.FirstOrDefaultAsync(p => p.Id == command.InventoryPartId, cancellationToken);
        if (inventoryPart is null) throw new InvalidOperationException("Inventory part could not be found.");


        var inventoryLocation = await DbContext.InventoryLocations.FirstOrDefaultAsync(l => l.Id == command.InventoryLocationId, cancellationToken);
        if (inventoryLocation is null) throw new InvalidOperationException("Inventory location could not be found.");
        if (!inventoryLocation.IsActive) throw new InvalidOperationException("Cannot receive inventory into an inactive location.");

        var condition = InventoryCondition.Serviceable;

        var stockBalance = await DbContext.InventoryStocks
            .FirstOrDefaultAsync(s =>
                s.InventoryPartId == inventoryPart.Id &&
                s.InventoryLocationId == inventoryLocation.Id &&
                s.Condition == condition,
                cancellationToken);

        int quantityBefore;
        int quantityAfter;

        if (stockBalance is not null)
        {
            quantityBefore = stockBalance.Quantity;

            var result = stockBalance.IncreaseQuantity(command.Quantity);

            if (!result.IsSuccess)
                throw new InvalidOperationException(result.ErrorMessage);

            quantityAfter = stockBalance.Quantity;
        }
        else
        {
            quantityBefore = 0;

            stockBalance = InventoryStock.Create(
                inventoryPart,
                command.Quantity,
                inventoryLocation,
                condition);

            DbContext.InventoryStocks.Add(stockBalance);

            quantityAfter = stockBalance.Quantity;
        }

        var transaction = Domain.Entities.InventoryTransaction.Create(
            inventoryPart.Id,
            InventoryTransactionType.Receipt,
            condition,
            command.Quantity,
            quantityBefore,
            quantityAfter,
            inventoryLocation.Id,
            workOrderId: null,
            performedByUserId: domainUserId,
            transactionDate: DateTime.UtcNow,
            reason: command.Reason);

        DbContext.InventoryTransactions.Add(transaction);

        await DbContext.SaveChangesAsync(cancellationToken);

        return new ReceiveInventoryCommandResult(transaction.Id);
    }
}

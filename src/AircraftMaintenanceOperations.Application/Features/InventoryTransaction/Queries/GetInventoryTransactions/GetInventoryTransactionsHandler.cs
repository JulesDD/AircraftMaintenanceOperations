namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Queries.GetInventoryTransactions;

public class GetInventoryTransactionsHandler(IAircraftMaintenanceDbContext dbContext) : IQueryHandler<GetInventoryTransactionsQuery, GetInventoryTransactionsQueryResult>
{
    public async Task<GetInventoryTransactionsQueryResult> Handle(GetInventoryTransactionsQuery query, CancellationToken cancellationToken)
    {
        var iTransactions = await dbContext.InventoryTransactions
            .AsNoTracking()
            .Where(it => it.InventoryPartId == query.InventoryPartId)
            .OrderByDescending(it => it.TransactionDate)
            .ToListAsync(cancellationToken);
        return new GetInventoryTransactionsQueryResult(iTransactions.Select(it => new InventoryTransactionDto
        (
            it.InventoryPartId,
            it.WorkOrderId,
            it.PerformedByUserId,
            it.Reason,
            it.TransactionDate,
            it.TransactionType,
            it.QuantityBefore,
            it.QuantityAfter,
            it.Quantity
        )));
    }
}

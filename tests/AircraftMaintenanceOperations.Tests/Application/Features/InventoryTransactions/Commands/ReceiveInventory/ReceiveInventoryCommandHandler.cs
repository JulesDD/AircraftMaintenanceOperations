namespace AircraftMaintenanceOperations.Tests.Application.Features.InventoryTransaction.Commands.ReceiveInventory;

[TestClass]
public class ReceiveInventoryCommandHandlerTests
{
    private AircraftMaintenanceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AircraftMaintenanceDbContext(options);
    }

    [TestMethod]
    public async Task Handle_WithValidQuantity_ShouldIncreaseStockAndCreateTransaction()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var performedByUserId = Guid.NewGuid();

        var inventoryPart = InventoryPart.Create(
            "BRK-001",
            "Brake Assembly",
            InventoryPartType.Rotable,
            10,
            5,
            "A-01");

        dbContext.InventoryParts.Add(inventoryPart);
        await dbContext.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(performedByUserId);

        var handler = new ReceiveInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new ReceiveInventoryCommand(inventoryPart.Id, 5, "Received shipment");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        var updatedInventory = await dbContext.InventoryParts.FirstAsync(x => x.Id == inventoryPart.Id);

        var transaction = await dbContext.InventoryTransactions.FirstOrDefaultAsync(x => x.Id == result.Id);

        Assert.AreEqual(15, updatedInventory.QuantityOnHand);

        Assert.IsNotNull(transaction);
        Assert.AreEqual(InventoryTransactionType.Receipt, transaction.TransactionType);
        Assert.AreEqual(5, transaction.Quantity);
        Assert.AreEqual(10, transaction.QuantityBefore);
        Assert.AreEqual(15, transaction.QuantityAfter);
        Assert.AreEqual(performedByUserId, transaction.PerformedByUserId);
        Assert.AreEqual("Received shipment", transaction.Reason);
    }

    [TestMethod]
    public async Task Handle_WithZeroQuantity_ShouldThrowAndNotCreateTransaction()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var inventoryPart = InventoryPart.Create(
            "BRK-002",
            "Brake Assembly",
            InventoryPartType.Rotable,
            10,
            5,
            "A-02");

        dbContext.InventoryParts.Add(inventoryPart);
        await dbContext.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(Guid.NewGuid());

        var handler = new ReceiveInventoryCommandHandler(dbContext, currentUser);

        var command = new ReceiveInventoryCommand(inventoryPart.Id, 0, "Invalid receipt");

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

        var updatedInventory = await dbContext.InventoryParts.FirstAsync(x => x.Id == inventoryPart.Id);

        var transactionCount = await dbContext.InventoryTransactions.CountAsync();

        Assert.AreEqual(10, updatedInventory.QuantityOnHand);
        Assert.AreEqual(0, transactionCount);
    }

    [TestMethod]
    public async Task Handle_WithNegativeQuantity_ShouldThrowAndNotCreateTransaction()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var inventoryPart = InventoryPart.Create(
            "BRK-003",
            "Brake Assembly",
            InventoryPartType.Rotable,
            10,
            5,
            "A-03");

        dbContext.InventoryParts.Add(inventoryPart);
        await dbContext.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(Guid.NewGuid());

        var handler = new ReceiveInventoryCommandHandler(dbContext, currentUser);

        var command = new ReceiveInventoryCommand(inventoryPart.Id, -5, "Invalid receipt");

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

        var updatedInventory = await dbContext.InventoryParts.FirstAsync(x => x.Id == inventoryPart.Id);

        var transactionCount = await dbContext.InventoryTransactions.CountAsync();

        Assert.AreEqual(10, updatedInventory.QuantityOnHand);
        Assert.AreEqual(0, transactionCount);
    }

    [TestMethod]
    public async Task Handle_WithMissingInventoryPart_ShouldThrowAndNotCreateTransaction()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var currentUser = new FakeCurrentUserService(Guid.NewGuid());

        var handler = new ReceiveInventoryCommandHandler(dbContext, currentUser);

        var command = new ReceiveInventoryCommand(Guid.NewGuid(), 5, "Missing part");

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

        var transactionCount = await dbContext.InventoryTransactions.CountAsync();

        Assert.AreEqual(0, transactionCount);
    }

    private sealed class FakeCurrentUserService : ICurrentUserService
    {
        public FakeCurrentUserService(Guid domainUserId)
        {
            DomainUserId = domainUserId;
        }

        public Guid UserId => DomainUserId;

        public Guid DomainUserId { get; }

        public IReadOnlyCollection<string> Roles => Array.Empty<string>();
    }
}
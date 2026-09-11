using AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.AdjustInventory;

namespace AircraftMaintenanceOperations.Tests.Application.Features.InventoryTransactions.Commands;

[TestClass]
public class AdjustInventoryCommandHandlerTests
{
    private AircraftMaintenanceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AircraftMaintenanceDbContext(options);
    }

    [TestMethod]
    public async Task Handle_WithPositiveAdjustment_ShouldIncreaseStockAndCreateTransaction()
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

        var handler = new AdjustInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new AdjustInventoryCommand(
            inventoryPart.Id,
            5,
            "Physical inventory count correction");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        var updatedInventory = await dbContext.InventoryParts
            .FirstAsync(x => x.Id == inventoryPart.Id);

        var transaction = await dbContext.InventoryTransactions
            .FirstOrDefaultAsync(x => x.Id == result.Id);

        Assert.AreEqual(15, updatedInventory.QuantityOnHand);

        Assert.IsNotNull(transaction);
        Assert.AreEqual(
            InventoryTransactionType.Adjustment,
            transaction.TransactionType);

        Assert.AreEqual(5, transaction.Quantity);
        Assert.AreEqual(10, transaction.QuantityBefore);
        Assert.AreEqual(15, transaction.QuantityAfter);
        Assert.AreEqual(
            performedByUserId,
            transaction.PerformedByUserId);

        Assert.IsNull(transaction.WorkOrderId);
        Assert.AreEqual(
            "Physical inventory count correction",
            transaction.Reason);
    }

    [TestMethod]
    public async Task Handle_WithNegativeAdjustment_ShouldDecreaseStockAndCreateTransaction()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var performedByUserId = Guid.NewGuid();

        var inventoryPart = InventoryPart.Create(
            "BRK-002",
            "Brake Assembly",
            InventoryPartType.Rotable,
            10,
            5,
            "A-02");

        dbContext.InventoryParts.Add(inventoryPart);
        await dbContext.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(performedByUserId);

        var handler = new AdjustInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new AdjustInventoryCommand(
            inventoryPart.Id,
            -4,
            "Damaged inventory correction");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        var updatedInventory = await dbContext.InventoryParts
            .FirstAsync(x => x.Id == inventoryPart.Id);

        var transaction = await dbContext.InventoryTransactions
            .FirstOrDefaultAsync(x => x.Id == result.Id);

        Assert.AreEqual(6, updatedInventory.QuantityOnHand);

        Assert.IsNotNull(transaction);
        Assert.AreEqual(
            InventoryTransactionType.Adjustment,
            transaction.TransactionType);

        Assert.AreEqual(4, transaction.Quantity);
        Assert.AreEqual(10, transaction.QuantityBefore);
        Assert.AreEqual(6, transaction.QuantityAfter);
        Assert.AreEqual(
            performedByUserId,
            transaction.PerformedByUserId);

        Assert.IsNull(transaction.WorkOrderId);
        Assert.AreEqual(
            "Damaged inventory correction",
            transaction.Reason);
    }

    [TestMethod]
    public async Task Handle_WithZeroAdjustment_ShouldThrowAndNotCreateTransaction()
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

        var handler = new AdjustInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new AdjustInventoryCommand(
            inventoryPart.Id,
            0,
            "Invalid zero adjustment");

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => handler.Handle(
                command,
                CancellationToken.None));

        var updatedInventory = await dbContext.InventoryParts
            .FirstAsync(x => x.Id == inventoryPart.Id);

        Assert.AreEqual(
            10,
            updatedInventory.QuantityOnHand);

        Assert.AreEqual(
            0,
            await dbContext.InventoryTransactions.CountAsync());
    }

    [TestMethod]
    public async Task Handle_WhenAdjustmentWouldMakeStockNegative_ShouldThrowAndNotCreateTransaction()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var inventoryPart = InventoryPart.Create(
            "BRK-004",
            "Brake Assembly",
            InventoryPartType.Rotable,
            5,
            2,
            "A-04");

        dbContext.InventoryParts.Add(inventoryPart);
        await dbContext.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(Guid.NewGuid());

        var handler = new AdjustInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new AdjustInventoryCommand(
            inventoryPart.Id,
            -10,
            "Invalid negative adjustment");

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => handler.Handle(
                command,
                CancellationToken.None));

        var updatedInventory = await dbContext.InventoryParts
            .FirstAsync(x => x.Id == inventoryPart.Id);

        Assert.AreEqual(
            5,
            updatedInventory.QuantityOnHand);

        Assert.AreEqual(
            0,
            await dbContext.InventoryTransactions.CountAsync());
    }

    [TestMethod]
    public async Task Handle_WithMissingInventoryPart_ShouldThrowAndNotCreateTransaction()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var currentUser = new FakeCurrentUserService(Guid.NewGuid());

        var handler = new AdjustInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new AdjustInventoryCommand(
            Guid.NewGuid(),
            5,
            "Missing inventory part");

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => handler.Handle(
                command,
                CancellationToken.None));

        Assert.AreEqual(
            0,
            await dbContext.InventoryTransactions.CountAsync());
    }

    private sealed class FakeCurrentUserService : ICurrentUserService
    {
        private readonly Guid _userId;
        public FakeCurrentUserService(Guid userId)
        {
            _userId = userId;
        }

        public Guid UserId => _userId;

        public Task<Guid> GetDomainUserIdAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_userId);
        }

        public IReadOnlyCollection<string> Roles =>
            Array.Empty<string>();
    }
}
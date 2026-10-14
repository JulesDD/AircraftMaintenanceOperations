namespace AircraftMaintenanceOperations.Tests.Application.Features.InventoryTransactions.Commands;

[TestClass]
public class ReceiveInventoryCommandHandlerTests
{
    public class TestEventPublisher : IEventPublisher
    {
        public List<object> PublishedEvents { get; } = new();

        public Task PublishAsync<T>(T @event, CancellationToken cancellationToken)
        {
            PublishedEvents.Add(@event!);
            return Task.CompletedTask;
        }
    }

    public class TestCurrentUserService : ICurrentUserService
    {
        private readonly Guid _domainUserId;

        public TestCurrentUserService(Guid domainUserId)
        {
            _domainUserId = domainUserId;
        }

        public Guid UserId => _domainUserId;

        public IReadOnlyCollection<string> Roles =>
            Array.Empty<string>();

        public Task<Guid> GetDomainUserIdAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_domainUserId);
        }
    }

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
        var eventPublisher = new TestEventPublisher();

        var handler = new ReceiveInventoryCommandHandler(
            dbContext,
            currentUser,
            eventPublisher);

        var command = new ReceiveInventoryCommand(inventoryPart.Id, 5, "Received shipment");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        var updatedInventory = await dbContext.InventoryParts.FirstAsync(x => x.Id == inventoryPart.Id);

        var transaction = await dbContext.InventoryTransactions.FirstOrDefaultAsync(x => x.Id == result.Id);

        Assert.AreEqual(15, updatedInventory.QuantityOnHand);

        Assert.AreEqual(1, eventPublisher.PublishedEvents.Count);
        var publishedEvent = eventPublisher.PublishedEvents.Single() as InventoryReceivedEvent;

        Assert.IsNotNull(publishedEvent);

        Assert.AreEqual(inventoryPart.Id, publishedEvent.InventoryPartId);
        Assert.AreEqual(5, publishedEvent.Quantity);
        Assert.AreEqual("Received shipment", publishedEvent.Reason);
        Assert.AreEqual(performedByUserId, publishedEvent.ReceivedByUserId);

        Assert.AreNotEqual(Guid.Empty, publishedEvent.EventId);
        Assert.AreNotEqual(default(DateTimeOffset), publishedEvent.OccurredAt);
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

        var currentUser = new TestCurrentUserService(Guid.NewGuid());
        var eventPublisher = new TestEventPublisher();

        var handler = new ReceiveInventoryCommandHandler(dbContext, currentUser, eventPublisher);

        var command = new ReceiveInventoryCommand(inventoryPart.Id, 0, "Invalid receipt");

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

        var updatedInventory = await dbContext.InventoryParts.FirstAsync(x => x.Id == inventoryPart.Id);

        var transactionCount = await dbContext.InventoryTransactions.CountAsync();

        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);


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

        var currentUser = new TestCurrentUserService(Guid.NewGuid());
        var eventPublisher = new TestEventPublisher();

        var handler = new ReceiveInventoryCommandHandler(dbContext, currentUser, eventPublisher);

        var command = new ReceiveInventoryCommand(inventoryPart.Id, -5, "Invalid receipt");

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

        var updatedInventory = await dbContext.InventoryParts.FirstAsync(x => x.Id == inventoryPart.Id);

        var transactionCount = await dbContext.InventoryTransactions.CountAsync();

        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);


        Assert.AreEqual(10, updatedInventory.QuantityOnHand);
        Assert.AreEqual(0, transactionCount);
    }

    [TestMethod]
    public async Task Handle_WithMissingInventoryPart_ShouldThrowAndNotCreateTransaction()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var currentUser = new TestCurrentUserService(Guid.NewGuid());
        var eventPublisher = new TestEventPublisher();

        var handler = new ReceiveInventoryCommandHandler(dbContext, currentUser, eventPublisher);

        var command = new ReceiveInventoryCommand(Guid.NewGuid(), 5, "Missing part");

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

        var transactionCount = await dbContext.InventoryTransactions.CountAsync();
        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);

        Assert.AreEqual(0, transactionCount);
    }


    private sealed class FakeCurrentUserService : ICurrentUserService
    {
        private readonly Guid _userId;

        public FakeCurrentUserService(Guid userId)
        {
            _userId = userId;
        }

        public Guid UserId => _userId;

        public Task<Guid> GetDomainUserIdAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_userId);
        }

        public IReadOnlyCollection<string> Roles =>
            Array.Empty<string>();
    }
}
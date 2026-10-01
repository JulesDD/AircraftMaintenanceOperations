namespace AircraftMaintenanceOperations.Tests.Application.Features.WorkOrders.Commands;

[TestClass]
public class CreatedWorkOrderCommandHandlerTests
{

    public class WorkOrderNumberGenerator : INumberGenerator
    {
        public Task<string> GenerateMaintenanceRequestNumberAsync()
        {
            return Task.FromResult("MR-TEST-001");
        }

        public Task<string> GenerateWorkOrderNumberAsync()
        {
            return Task.FromResult("WO-TEST-001");
        }
    }
    public class TestEventPublisher : IEventPublisher
    {
        public List<object> PublishedEvents { get; } = new List<object>();
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

        public IReadOnlyCollection<string> Roles => Array.Empty<string>();

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
    public async Task Handle_ShouldCreateWorkOrder_AndPublishEvent_WhenValidCommand()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var aircraftId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var maintenanceRequest = MaintenanceRequest.Create(
            "MR-TEST-001",
            "Brake Replacement",
            aircraftId,
            "Replace aircraft brakes",
            userId,
            DateTime.UtcNow.AddDays(7));

        maintenanceRequest.Start();

        dbContext.MaintenanceRequests.Add(maintenanceRequest);
        await dbContext.SaveChangesAsync();

        var numberGenerator = new WorkOrderNumberGenerator();
        var currentUserService = new TestCurrentUserService(userId);
        var eventPublisher = new TestEventPublisher();

        var handler = new CreateWorkOrderHandler(
            dbContext,
            numberGenerator,
            eventPublisher,
            currentUserService);

        var command = new CreateWorkOrderCommand(
            maintenanceRequest.Id,
            aircraftId,
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Replace brake components");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreNotEqual(Guid.Empty, result.Id);

        var createdWorkOrder = await dbContext.WorkOrders.FindAsync(result.Id);

        Assert.IsNotNull(createdWorkOrder);
        Assert.AreEqual(maintenanceRequest.Id, createdWorkOrder!.MaintenanceRequestId);
        Assert.AreEqual(aircraftId, createdWorkOrder.AircraftId);
        Assert.AreEqual(WorkOrderStatus.Open, createdWorkOrder.WorkOrderStatus);
        Assert.AreEqual(MaintenancePriority.Medium, createdWorkOrder.WorkOrderPriority);

        // Event assertions
        Assert.AreEqual(1, eventPublisher.PublishedEvents.Count);

        var publishedEvent = eventPublisher.PublishedEvents.Single() as WorkOrderCreatedEvent;

        Assert.IsNotNull(publishedEvent);
        Assert.AreEqual(maintenanceRequest.Id, publishedEvent!.MaintenanceRequestId);
        Assert.AreEqual(result.Id, publishedEvent.WorkOrderId);
        Assert.AreEqual(aircraftId, publishedEvent.AircraftId);
        Assert.AreEqual(userId, publishedEvent.CreatedByUserId);
        Assert.AreNotEqual(Guid.Empty, publishedEvent.EventId);
        Assert.IsTrue(publishedEvent.OccurredAt <= DateTimeOffset.UtcNow);
    }

    [TestMethod]
    public async Task Handle_ShouldThrow_WhenWorkOrderAlreadyExistsForMaintenanceRequest()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var aircraftId = Guid.NewGuid();
        
        var userId = Guid.NewGuid();

        var maintenanceRequest = MaintenanceRequest.Create(
            "MR-TEST-001",
            "Brake Replacement",
            aircraftId,
            "Replace aircraft brakes",
            userId,
            DateTime.UtcNow.AddDays(7));

        maintenanceRequest.Start();

        dbContext.MaintenanceRequests.Add(maintenanceRequest);
        await dbContext.SaveChangesAsync();

        var workOrder = WorkOrder.Create(
            "WO-TEST-001",
            maintenanceRequest.Id,
            aircraftId,
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Creating a WO");

        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        var existingWorkOrder = await dbContext.WorkOrders.FirstOrDefaultAsync(x => x.MaintenanceRequestId == maintenanceRequest.Id);
        
        var numberGenerator = new WorkOrderNumberGenerator();
        
        var currentUserService = new TestCurrentUserService(userId);
        
        var eventPublisher = new TestEventPublisher();

        var hasWorkOrderForRequest = await dbContext.WorkOrders.AnyAsync(x => x.MaintenanceRequestId == maintenanceRequest.Id);

        var hasOpenWorkOrderForAircraft = await dbContext.WorkOrders.AnyAsync(x => x.AircraftId == aircraftId && x.WorkOrderStatus != WorkOrderStatus.Completed && x.WorkOrderStatus != WorkOrderStatus.Archived);

        var handler = new CreateWorkOrderHandler(
            dbContext,
            numberGenerator,
            eventPublisher,
            currentUserService);

        var command = new CreateWorkOrderCommand(
            maintenanceRequest.Id,
            aircraftId,
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Replace brake components");


        // Act
        var exceptionCaught = false;
        string? exceptionMessage = null;

        try
        {
            await handler.Handle(command, CancellationToken.None);
        }
        catch (InvalidOperationException ex)
        {
            exceptionCaught = true;
            exceptionMessage = ex.Message;
        }

        // Assert
        Assert.IsNotNull(existingWorkOrder);
        Assert.AreEqual(maintenanceRequest.Id, existingWorkOrder!.MaintenanceRequestId);
        Assert.IsTrue(exceptionCaught);
        Assert.IsTrue(hasWorkOrderForRequest);
        Assert.IsTrue(hasOpenWorkOrderForAircraft);
        Assert.AreEqual($"Work order for maintenance request with ID {maintenanceRequest.Id} already exists.", exceptionMessage);
        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);
    }

    [TestMethod]
    public async Task Handle_ShouldThrow_WhenAircraftAlreadyHasOpenWorkOrder()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var aircraftId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var maintenanceRequest = MaintenanceRequest.Create(
            "MR-TEST-001",
            "Brake Replacement",
            aircraftId,
            "Replace aircraft brakes",
            userId,
            DateTime.UtcNow.AddDays(7));

        var maintenanceRequest2 = MaintenanceRequest.Create(
            "MR-TEST-002",
            "Brake Replacement",
            aircraftId,
            "Replace aircraft brakes",
            userId,
            DateTime.UtcNow.AddDays(7));

        maintenanceRequest.Start();
        maintenanceRequest2.Start();

        dbContext.MaintenanceRequests.Add(maintenanceRequest);
        dbContext.MaintenanceRequests.Add(maintenanceRequest2);
        await dbContext.SaveChangesAsync();

        var workOrder = WorkOrder.Create(
            "WO-TEST-001",
            maintenanceRequest2.Id,
            aircraftId,
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Creating a WO");

        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        var numberGenerator = new WorkOrderNumberGenerator();
        var currentUserService = new TestCurrentUserService(userId);
        var eventPublisher = new TestEventPublisher();
        var hasOpenWorkOrderForAircraft = await dbContext.WorkOrders.AnyAsync(x => x.AircraftId == aircraftId && x.WorkOrderStatus != WorkOrderStatus.Completed && x.WorkOrderStatus != WorkOrderStatus.Archived);

        var handler = new CreateWorkOrderHandler(
            dbContext,
            numberGenerator,
            eventPublisher,
            currentUserService);

        var command = new CreateWorkOrderCommand(
            maintenanceRequest.Id,
            aircraftId,
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Replace brake components");

        var exceptionCaught = false;
        string? exceptionMessage = null;

        try
        {
            await handler.Handle(command, CancellationToken.None);
        }
        catch (InvalidOperationException ex)
        {
            exceptionCaught = true;
            exceptionMessage = ex.Message;
        }

        Assert.IsTrue(exceptionCaught);
        Assert.IsTrue(hasOpenWorkOrderForAircraft);
        Assert.AreEqual($"Aircraft with ID {aircraftId} already has an open work order.", exceptionMessage);
        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);

    }

    [TestMethod]
    public async Task Handle_ShouldThrow_WhenMaintenanceRequestDoesNotExist()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var aircraftId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var numberGenerator = new WorkOrderNumberGenerator();
        var currentUserService = new TestCurrentUserService(userId);
        var eventPublisher = new TestEventPublisher();

        var handler = new CreateWorkOrderHandler(
            dbContext,
            numberGenerator,
            eventPublisher,
            currentUserService);

        var command = new CreateWorkOrderCommand(
            Guid.NewGuid(),
            aircraftId,
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Replace brake components");

        var exceptionCaught = false;
        string? exceptionMessage = null;

        try
        {
            await handler.Handle(command, CancellationToken.None);
        }
        catch (InvalidOperationException ex)
        {
            exceptionCaught = true;
            exceptionMessage = ex.Message;
        }

        Assert.IsTrue(exceptionCaught);
        Assert.AreEqual($"Maintenance request not found.", exceptionMessage);
        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);
    }

    [TestMethod]
    public async Task Handle_ShouldThrow_WhenMaintenanceRequestIsNotInProgress()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var aircraftId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Create MR in a status other than InProgress (e.g., Pending)
        var maintenanceRequest = MaintenanceRequest.Create(
            "MR-TEST-001",
            "Brake Replacement",
            aircraftId,
            "Replace aircraft brakes",
            userId,
            DateTime.UtcNow.AddDays(7));

        dbContext.MaintenanceRequests.Add(maintenanceRequest);
        await dbContext.SaveChangesAsync();

        var numberGenerator = new WorkOrderNumberGenerator();
        var currentUserService = new TestCurrentUserService(userId);
        var eventPublisher = new TestEventPublisher();

        var handler = new CreateWorkOrderHandler(
            dbContext,
            numberGenerator,
            eventPublisher,
            currentUserService);

        var command = new CreateWorkOrderCommand(
            maintenanceRequest.Id,
            aircraftId,
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Replace brake components");

        // Act
        var exceptionCaught = false;
        string? exceptionMessage = null;

        try
        {
            await handler.Handle(command, CancellationToken.None);
        }
        catch (InvalidOperationException ex)
        {
            exceptionCaught = true;
            exceptionMessage = ex.Message;
        }

        Assert.IsTrue(exceptionCaught);
        Assert.AreEqual("Work orders can only be created from InProgress requests.", exceptionMessage);
        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);
    }

    [TestMethod]
    public async Task Handle_ShouldNotPublishEvent_WhenWorkOrderCreationFails()
    {
    }
}

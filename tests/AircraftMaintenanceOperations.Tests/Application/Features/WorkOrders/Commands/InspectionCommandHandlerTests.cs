namespace AircraftMaintenanceOperations.Tests.Application.Features.WorkOrders.Commands;

[TestClass]
public class InspectionCommandHandlerTests
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
    public async Task Handle_InspectionCommand_ReturnsSuccessResult()
    {
        await using var dbContext = CreateDbContext();

        // Arrange
        var technician = Technician.Create(
            "EMP123",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            CertificationLevel.Junior,
            3);

        var workOrder = WorkOrder.Create(
            "WO-TEST-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Create a test work order for unit testing.");

        
        var assignResult = workOrder.AssignTechnician(technician, "Assigning technician for testing.");
        Assert.IsTrue(assignResult.IsSuccess, assignResult.ErrorMessage);

        var startResult = workOrder.InProgress("Maintenance work completed.");
        Assert.IsTrue(startResult.IsSuccess, startResult.ErrorMessage);
        Assert.AreEqual(WorkOrderStatus.InProgress, workOrder.WorkOrderStatus);

        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        var inspectionCommand = new InspectionCommand
        (
            workOrder.Id,
            "Initial inspection completed. All systems functional."
        );

        var eventPublisher = new TestEventPublisher();
        var currentUserService = new TestCurrentUserService(technician.Id);

        var handler = new InspectionCommandHandler(dbContext, currentUserService, eventPublisher);

        // Act
        var result = await handler.Handle(inspectionCommand, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsInspected);
        Assert.AreEqual("Initial inspection completed. All systems functional.", workOrder.LaborNotes);
        Assert.AreEqual(1, eventPublisher.PublishedEvents.Count);

        var publishedEvent = eventPublisher.PublishedEvents.Single() as WorkOrderSentToInspectionEvent;
        Assert.IsNotNull(publishedEvent);

        Assert.AreEqual(workOrder.Id, publishedEvent!.WorkOrderId);
        Assert.AreEqual(currentUserService.UserId, publishedEvent.SentByUserId);
        Assert.AreNotEqual(default, publishedEvent.OccurredAt);
        Assert.AreNotEqual(Guid.Empty, publishedEvent.EventId);
    }
}

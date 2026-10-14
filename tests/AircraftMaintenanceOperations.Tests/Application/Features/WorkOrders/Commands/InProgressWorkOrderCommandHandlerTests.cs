namespace AircraftMaintenanceOperations.Tests.Application.Features.WorkOrders.Commands;

[TestClass]
public class InProgressWorkOrderCommandHandlerTests
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
    public async Task InProgressCommandHandler_Should_Start_WorkOrder_And_Publish_Event()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var aircraftId = Guid.NewGuid();

        var technician = Technician.Create(
            "TECH-001",
            "John",
            "Smith",
            "john.smith@test.com",
            "555-0100",
            CertificationLevel.Junior,
            3);

        var workOrder = WorkOrder.Create(
            "WO-TEST-001",
            Guid.NewGuid(),
            aircraftId,
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Create a test work order for unit testing.");

        dbContext.Technicians.Add(technician);
        dbContext.WorkOrders.Add(workOrder);

        await dbContext.SaveChangesAsync();

        // Assign the technician so the Work Order can enter InProgress.
        var assignmentResult = workOrder.AssignTechnician(
            technician,
            "Assign technician for unit testing.");

        Assert.IsTrue(
            assignmentResult.IsSuccess,
            assignmentResult.ErrorMessage);

        await dbContext.SaveChangesAsync();

        // The assigned technician is the current user.
        var currentUserService =
            new TestCurrentUserService(technician.Id);

        var eventPublisher = new TestEventPublisher();

        var handler = new InProgressCommandHandler(
            dbContext,
            currentUserService,
            eventPublisher);

        var command = new InProgressCommand(
            workOrder.Id,
            technician.Id,
            "Starting work on the work order.");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert: the operation succeeded.
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsInProgress);

        // Assert: exactly one event was published.
        Assert.AreEqual(
            1,
            eventPublisher.PublishedEvents.Count);

        var publishedEvent = eventPublisher.PublishedEvents
            .OfType<WorkOrderInProgressEvent>()
            .Single();

        // Assert: the event contains the correct details.
        Assert.AreEqual(
            workOrder.Id,
            publishedEvent.WorkOrderId);

        Assert.AreEqual(
            technician.Id,
            publishedEvent.StartedByUserId);

        Assert.AreNotEqual(
            Guid.Empty,
            publishedEvent.EventId);

        Assert.AreNotEqual(
            default(DateTimeOffset),
            publishedEvent.OccurredAt);
    }
}

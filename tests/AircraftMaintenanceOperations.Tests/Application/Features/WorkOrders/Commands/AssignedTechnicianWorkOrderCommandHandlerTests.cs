using AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.AssignTechnicianCommand;

namespace AircraftMaintenanceOperations.Tests.Application.Features.WorkOrders.Commands;

[TestClass]
public class AssignedTechnicianWorkOrderCommandHandlerTests
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
    public async Task Handle_AssignTechnicianCommand_ReturnsSuccessResult()
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

        dbContext.Technicians.Add(technician);
        dbContext.WorkOrders.Add(workOrder);

        var testEventPublisher = new TestEventPublisher();
        var testCurrentUserService = new TestCurrentUserService(Guid.NewGuid());

        var handler = new AssignTechnicianCommandHandler(
            dbContext,
            testEventPublisher,
            testCurrentUserService);

        var command = new AssignTechnicianCommand(
            workOrder.Id,
            technician.Id,
            "Assigning technician for unit testing.");

       

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(technician.Id, workOrder.AssignedTechnicianId);
        Assert.AreEqual(1, testEventPublisher.PublishedEvents.Count);
        
        var publishedEvent = testEventPublisher.PublishedEvents.Single() as TechnicianAssignedToWorkOrderEvent;
        Assert.IsNotNull(publishedEvent);

        Assert.AreEqual(workOrder.Id, publishedEvent.WorkOrderId);
        Assert.AreEqual(technician.Id, publishedEvent.TechnicianId);
        Assert.AreNotEqual( Guid.Empty, publishedEvent.EventId);
        Assert.AreNotEqual(default, publishedEvent.OccurredAt);
        Assert.AreEqual(testCurrentUserService.UserId, publishedEvent.AssignedByUserId);
    }

    [TestMethod]
    public async Task Handle_AssignTechnicianCommand_ReturnsFailureResult_WhenTechnicianNotFound()
    {
        await using var dbContext = CreateDbContext();
        // Arrange
        var workOrder = WorkOrder.Create(
            "WO-TEST-002",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Create a test work order for unit testing.");

        dbContext.WorkOrders.Add(workOrder);
        var testEventPublisher = new TestEventPublisher();
        var testCurrentUserService = new TestCurrentUserService(Guid.NewGuid());
        var handler = new AssignTechnicianCommandHandler(
            dbContext,
            testEventPublisher,
            testCurrentUserService);
        var command = new AssignTechnicianCommand(
            workOrder.Id,
            Guid.NewGuid(), // Non-existent technician ID
            "Assigning non-existent technician for unit testing.");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(0, testEventPublisher.PublishedEvents.Count);
        Assert.AreEqual("Technician or work order not found.", result.ErrorMessage);
    }
}

using AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.ConcludedWorkOrderCommand;

namespace AircraftMaintenanceOperations.Tests.Application.Features.WorkOrders.Commands;

[TestClass]
public class ConcludedWorkOrderCommandHandlerTests
{
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
    public async Task ConcludedWorkOrderCommandHandler_Should_Conclude_WorkOrder_And_Publish_Event()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var technician = Technician.Create(
            "TECH-001",
            "John",
            "Smith",
            "john.smith@test.com",
            "555-0100",
            CertificationLevel.Junior,
            3);
        var currentUserService = new TestCurrentUserService(technician.Id);

        var workOrder = WorkOrder.Create(
            "WO-TEST-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial labor notes");

        dbContext.WorkOrders.Add(workOrder);
        dbContext.Technicians.Add(technician);

        await dbContext.SaveChangesAsync();

        var command = new ConcludedWorkOrderCommand(workOrder.Id, "Work completed successfully.Please review the attached report for details.");
        Assert.AreEqual(workOrder.Id, command.WorkOrderId);

        var assignedResult = workOrder.AssignTechnician(technician, "Assigning technician for work order.", Guid.NewGuid());
        Assert.IsTrue(assignedResult.IsSuccess, assignedResult.ErrorMessage);

        workOrder.InProgress("Starting work on the order.");
        Assert.AreEqual(WorkOrderStatus.InProgress, workOrder.WorkOrderStatus);

        await dbContext.SaveChangesAsync();

        var eventPublisher = new TestEventPublisher();
        var handler = new ConcludedWorkOrderHandler(dbContext, eventPublisher, currentUserService);
        var savedWorkOrder = await dbContext.WorkOrders.FindAsync(workOrder.Id);

        Assert.IsNotNull(savedWorkOrder);
        Assert.AreEqual(workOrder.Id, command.WorkOrderId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        Assert.IsTrue(result.IsConcluded, result.Message);
        Assert.AreEqual(1, eventPublisher.PublishedEvents.Count);

        var publishedEvents = eventPublisher.PublishedEvents.OfType<WorkOrderConcludedEvent>().Single();

        // Assert
        Assert.AreEqual(workOrder.Id, publishedEvents.WorkOrderId);
        Assert.AreEqual("Work completed successfully.Please review the attached report for details.", publishedEvents.LaborNotes);
        Assert.AreEqual(WorkOrderStatus.Concluded, workOrder.WorkOrderStatus);
        Assert.AreNotEqual(Guid.Empty,publishedEvents.EventId);
        Assert.AreNotEqual(default(DateTimeOffset), publishedEvents.OccurredAt);
        Assert.AreEqual(technician.Id, publishedEvents.ConcludedByUserId);
    }
}
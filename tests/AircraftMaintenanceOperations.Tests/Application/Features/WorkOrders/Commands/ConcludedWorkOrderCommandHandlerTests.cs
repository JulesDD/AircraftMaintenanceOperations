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

    [TestMethod]
    public async Task ConcludedWorkOrderCommandHandler_Should_Fail_No_WorkOrder_Found()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var command = new ConcludedWorkOrderCommand(Guid.NewGuid(), "Work completed successfully.");
        var currentUserService = new TestCurrentUserService(Guid.NewGuid());
        var eventPublisher = new TestEventPublisher();

        var handler = new ConcludedWorkOrderHandler(
            dbContext,
            eventPublisher,
            currentUserService);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsConcluded);
        Assert.AreEqual("Work order not found.", result.Message);
        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);
    }

    [TestMethod]
    public async Task ConcludedWorkOrderCommandHandler_Should_Fail_When_WorkOrder_Is_Not_InProgress()
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

        await dbContext.SaveChangesAsync();

        var eventPublisher = new TestEventPublisher();
        var handler = new ConcludedWorkOrderHandler(dbContext, eventPublisher, currentUserService);
        var savedWorkOrder = await dbContext.WorkOrders.FindAsync(workOrder.Id);

        Assert.IsNotNull(savedWorkOrder);
        Assert.AreEqual(workOrder.Id, command.WorkOrderId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        Assert.IsFalse(result.IsConcluded, result.Message);
        Assert.AreEqual("Only work orders in progress can be moved to concluded.", result.Message);
        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);
        Assert.AreEqual(WorkOrderStatus.Assigned, workOrder.WorkOrderStatus);
    }

    [TestMethod]
    public async Task ConcludedWorkOrderCommandHandler_Should_Fail_When_User_Is_Not_Authorized()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var technicianA = Technician.Create(
            "TECH-001",
            "John",
            "Smith",
            "john.smith@test.com",
            "555-0100",
            CertificationLevel.Junior,
            3);

        var technicianB = Technician.Create(
            "TECH-002",
            "Jane",
            "Doe",
            "jane.doe@test.com",
            "555-0101",
            CertificationLevel.Senior,
            5);
        var currentUserService = new TestCurrentUserService(technicianB.Id);

        var workOrder = WorkOrder.Create(
            "WO-TEST-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial labor notes");

        dbContext.WorkOrders.Add(workOrder);
        dbContext.Technicians.Add(technicianA);
        dbContext.Technicians.Add(technicianB);

        await dbContext.SaveChangesAsync();

        var command = new ConcludedWorkOrderCommand(workOrder.Id, "Work completed successfully. Please review the attached report for details.");
        Assert.AreEqual(workOrder.Id, command.WorkOrderId);

        var assignedResult = workOrder.AssignTechnician(technicianA, "Assigning technician for work order.", Guid.NewGuid());
        Assert.IsTrue(assignedResult.IsSuccess, assignedResult.ErrorMessage);

        workOrder.InProgress("Starting work on the order.");
        Assert.AreEqual(WorkOrderStatus.InProgress, workOrder.WorkOrderStatus);

        await dbContext.SaveChangesAsync();

        var eventPublisher = new TestEventPublisher();
        var handler = new ConcludedWorkOrderHandler(dbContext, eventPublisher, currentUserService);
        Assert.AreEqual(WorkOrderStatus.InProgress, workOrder.WorkOrderStatus);
        Assert.AreEqual(workOrder.Id, command.WorkOrderId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        Assert.IsFalse(result.IsConcluded);
        Assert.AreEqual("You are not authorized to conclude this work order.", result.Message);
        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);
    }
}
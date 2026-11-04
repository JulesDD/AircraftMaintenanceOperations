using AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.OnHoldWorkOrderCommand;

namespace AircraftMaintenanceOperations.Tests.Application.Features.WorkOrders.Commands;

[TestClass]
public class OnHoldWorkOrderCommandHandlerTests
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
        private readonly IReadOnlyCollection<string> _roles;
        public TestCurrentUserService(Guid domainUserId, IReadOnlyCollection<string> roles = null)
        {
            _domainUserId = domainUserId;
            _roles = roles ?? Array.Empty<string>();
        }

        public Guid UserId => _domainUserId;

        public IReadOnlyCollection<string> Roles => _roles;

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
    public async Task Handle_AssignedTechnicianPlacesWorkOrderOnHold()
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

        dbContext.Technicians.Add(technician);
        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        var command = new OnHoldWorkOrderCommand(workOrder.Id, HoldReason.Parts, "Test Hold Notes");
        Assert.AreEqual(workOrder.Id, command.WorkOrderId);

        var assignedResult = workOrder.AssignTechnician(technician, "Assigning technician for work order.", Guid.NewGuid());
        Assert.IsTrue(assignedResult.IsSuccess, assignedResult.ErrorMessage);

        workOrder.InProgress("Starting work on the order.");
        Assert.AreEqual(WorkOrderStatus.InProgress, workOrder.WorkOrderStatus);

        await dbContext.SaveChangesAsync();

        var eventPublisher = new TestEventPublisher();
        var handler = new OnHoldWorkOrderCommandHandler(dbContext, currentUserService, eventPublisher);
        var savedWorkOrder = await dbContext.WorkOrders.FindAsync(workOrder.Id);

        Assert.IsNotNull(savedWorkOrder);
        Assert.AreEqual(workOrder.Id,command.WorkOrderId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsOnHold);
        Assert.AreEqual(1, eventPublisher.PublishedEvents.Count);

        var publishedEvent = eventPublisher.PublishedEvents.FirstOrDefault() as WorkOrderOnHoldEvent;

        Assert.AreEqual(workOrder.Id, publishedEvent?.WorkOrderId);
        Assert.AreEqual(HoldReason.Parts, publishedEvent?.HoldReason);
        Assert.AreEqual("Test Hold Notes", publishedEvent?.HoldNotes);
        Assert.AreEqual(technician.Id, publishedEvent?.OnHoldByUserId);
        Assert.AreNotEqual(Guid.Empty, publishedEvent?.EventId);
        Assert.AreNotEqual(default(DateTimeOffset), publishedEvent?.OccurredAt);
    }

    [TestMethod]
    public async Task Handle_SupervisorPlacesWorkOrderOnHold()
    {
        await using var dbContext = CreateDbContext();

        var technician = Technician.Create(
             "TECH-001",
             "John",
             "Smith",
             "john.smith@test.com",
             "555-0100",
             CertificationLevel.Junior,
             3);

        var supervisor = AircraftMaintenanceOperations.Domain.Entities.User.Create(
             "SUp-001",
             "Jane",
             "Doe",
             "jane.doe@test.com",
             "555-0101",
             Role.MaintenanceSupervisor);
        var currentUserService = new TestCurrentUserService(supervisor.Id, new[] { "MaintenanceSupervisor" });

        var workOrder = WorkOrder.Create(
            "WO-TEST-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial labor notes");

        dbContext.Technicians.Add(technician);
        dbContext.Users.Add(supervisor);
        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        var command = new OnHoldWorkOrderCommand(workOrder.Id, HoldReason.Parts, "Test Hold Notes");
        Assert.AreEqual(workOrder.Id, command.WorkOrderId);

        var assignedResult = workOrder.AssignTechnician(technician, "Assigning technician for work order.", Guid.NewGuid());
        Assert.IsTrue(assignedResult.IsSuccess, assignedResult.ErrorMessage);

        workOrder.InProgress("Starting work on the order.");
        Assert.AreEqual(WorkOrderStatus.InProgress, workOrder.WorkOrderStatus);

        await dbContext.SaveChangesAsync();

        var eventPublisher = new TestEventPublisher();
        var handler = new OnHoldWorkOrderCommandHandler(dbContext, currentUserService, eventPublisher);
        var savedWorkOrder = await dbContext.WorkOrders.FindAsync(workOrder.Id);

        Assert.IsNotNull(savedWorkOrder);
        Assert.AreEqual(workOrder.Id, command.WorkOrderId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsOnHold);
        Assert.AreEqual(1, eventPublisher.PublishedEvents.Count);

        var publishedEvent = eventPublisher.PublishedEvents.FirstOrDefault() as WorkOrderOnHoldEvent;

        Assert.AreEqual(workOrder.Id, publishedEvent?.WorkOrderId);
        Assert.AreEqual(HoldReason.Parts, publishedEvent?.HoldReason);
        Assert.AreEqual("Test Hold Notes", publishedEvent?.HoldNotes);
        Assert.AreEqual(supervisor.Id, publishedEvent?.OnHoldByUserId);
        Assert.AreNotEqual(Guid.Empty, publishedEvent?.EventId);
        Assert.AreNotEqual(default(DateTimeOffset), publishedEvent?.OccurredAt);
    }

    [TestMethod]
    public async Task Handle_UnassignedTechnicianCannotPlaceWorkOrderOnHold()
    {
        await using var dbContext = CreateDbContext();

        var technician = Technician.Create(
          "TECH-001",
          "John",
          "Smith",
          "john.smith@test.com",
          "555-0100",
          CertificationLevel.Junior,
          3);
        
        var technician2 = Technician.Create(
          "TECH-002",
          "Alice",
          "Johnson",
          "alice.johnson@test.com",
          "555-0102",
          CertificationLevel.Senior,
          5);
        var currentUserService = new TestCurrentUserService(technician2.Id);

        var workOrder = WorkOrder.Create(
            "WO-TEST-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial labor notes");

        dbContext.Technicians.Add(technician);
        dbContext.Technicians.Add(technician2);
        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        var command = new OnHoldWorkOrderCommand(workOrder.Id, HoldReason.Parts, "Test Hold Notes");
        Assert.AreEqual(workOrder.Id, command.WorkOrderId);

        var assignedResult = workOrder.AssignTechnician(technician, "Assigning technician for work order.", Guid.NewGuid());
        Assert.IsTrue(assignedResult.IsSuccess, assignedResult.ErrorMessage);

        workOrder.InProgress("Starting work on the order.");
        Assert.AreEqual(WorkOrderStatus.InProgress, workOrder.WorkOrderStatus);

        await dbContext.SaveChangesAsync();

        var eventPublisher = new TestEventPublisher();
        var handler = new OnHoldWorkOrderCommandHandler(dbContext, currentUserService, eventPublisher);
        var savedWorkOrder = await dbContext.WorkOrders.FindAsync(workOrder.Id);

        Assert.IsNotNull(savedWorkOrder);
        Assert.AreEqual(workOrder.Id, command.WorkOrderId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsOnHold, result.Message);
        Assert.AreEqual("You are not authorized to place this work order on hold.", result.Message);
        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);
        Assert.AreEqual(WorkOrderStatus.InProgress, savedWorkOrder?.WorkOrderStatus);
    }

    [TestMethod]
    public async Task Handle_MissingWorkOrderNumberCannotPlaceWorkOrderOnHold()
    {
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

        dbContext.Technicians.Add(technician);
        await dbContext.SaveChangesAsync();

        var command = new OnHoldWorkOrderCommand(Guid.NewGuid(), HoldReason.Parts, "Test Hold Notes");
        var eventPublisher = new TestEventPublisher();
        var handler = new OnHoldWorkOrderCommandHandler(dbContext, currentUserService, eventPublisher);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsOnHold, result.Message);
        Assert.AreEqual("Work order not found.", result.Message);
        Assert.AreEqual(0, eventPublisher.PublishedEvents.Count);
    }

}

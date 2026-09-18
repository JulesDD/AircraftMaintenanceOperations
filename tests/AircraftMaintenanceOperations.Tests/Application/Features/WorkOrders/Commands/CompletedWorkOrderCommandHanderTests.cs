namespace AircraftMaintenanceOperations.Tests.Application.Features.WorkOrders.Commands;

[TestClass]
public class CompletedWorkOrderCommandHanderTests
{
    private AircraftMaintenanceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AircraftMaintenanceDbContext(options);
    }

    [TestMethod]
    public async Task Handle_ValidWorkOrder_ReturnsSuccess()
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

        dbContext.Technicians.Add(technician);

        var workOrder = WorkOrder.Create(
            "WO-TEST-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial maintenance note");

        var assignResult = workOrder.AssignTechnician(technician, "Technician assigned");
        Assert.IsTrue(assignResult.IsSuccess);

        var inProgressResult = workOrder.InProgress("Maintenance work started");
        Assert.IsTrue(inProgressResult.IsSuccess);

        var inspectionResult = workOrder.Inspection("Inspection completed");
        Assert.IsTrue(inspectionResult.IsSuccess);

        dbContext.WorkOrders.Add(workOrder);

        await dbContext.SaveChangesAsync();
        var handler = new CompletedWorkOrderCommandHandler(dbContext);
        var command = new CompletedWorkOrderCommand(
            workOrder.Id,
            "Work order completed successfully",
            4.5m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var savedWorkOrder = await dbContext.WorkOrders.FirstAsync(w => w.Id == workOrder.Id);

        Assert.AreEqual(WorkOrderStatus.Completed, savedWorkOrder.WorkOrderStatus);
        Assert.AreEqual(4.5m, savedWorkOrder.LaborHours);
        Assert.AreEqual("Work order completed successfully", savedWorkOrder.LaborNotes);
        Assert.IsNotNull(savedWorkOrder.ActualCompletionDate);
    }

    [TestMethod]
    public async Task Handle_WorkOrderDoesNotExist_ReturnsFailure()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var handler = new CompletedWorkOrderCommandHandler(dbContext);

        var command = new CompletedWorkOrderCommand(
            Guid.NewGuid(),
            "Attempted completion",
            4.5m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Unable to find work order", result.ErrorMessage);
    }

    [TestMethod]
    public async Task Handle_MissingLaborNotes_ReturnsFailure()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var technician = Technician.Create(
            "TECH-002",
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-0101",
            CertificationLevel.Junior,
            3);

        dbContext.Technicians.Add(technician);

        var workOrder = WorkOrder.Create(
            "WO-TEST-002",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial maintenance note");

        var assignResult = workOrder.AssignTechnician(
            technician,
            "Technician assigned");

        Assert.IsTrue(assignResult.IsSuccess);

        var inProgressResult = workOrder.InProgress(
            "Maintenance work started");

        Assert.IsTrue(inProgressResult.IsSuccess);

        var inspectionResult = workOrder.Inspection(
            "Inspection completed");

        Assert.IsTrue(inspectionResult.IsSuccess);

        dbContext.WorkOrders.Add(workOrder);

        await dbContext.SaveChangesAsync();

        var handler = new CompletedWorkOrderCommandHandler(dbContext);

        var command = new CompletedWorkOrderCommand(
            workOrder.Id,
            "",
            4.5m);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsSuccess);

        Assert.AreEqual(
            "A status note is required.",
            result.ErrorMessage);
    }

    [TestMethod]
    public async Task Handle_ZeroLaborHours_ReturnsFailure()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var technician = Technician.Create(
            "TECH-003",
            "Mike",
            "Johnson",
            "mike.johnson@test.com",
            "555-0102",
            CertificationLevel.Junior,
            4);

        dbContext.Technicians.Add(technician);

        var workOrder = WorkOrder.Create(
            "WO-TEST-003",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial maintenance note");

        var assignResult = workOrder.AssignTechnician(
            technician,
            "Technician assigned");

        Assert.IsTrue(assignResult.IsSuccess);

        var inProgressResult = workOrder.InProgress(
            "Maintenance work started");

        Assert.IsTrue(inProgressResult.IsSuccess);

        var inspectionResult = workOrder.Inspection(
            "Inspection completed");

        Assert.IsTrue(inspectionResult.IsSuccess);

        dbContext.WorkOrders.Add(workOrder);

        await dbContext.SaveChangesAsync();

        var handler = new CompletedWorkOrderCommandHandler(dbContext);

        var command = new CompletedWorkOrderCommand(
            workOrder.Id,
            "Work order completed",
            0m);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsSuccess);

        Assert.AreEqual(
            "Labor hours must be greater than zero.",
            result.ErrorMessage);
    }

    [TestMethod]
    public async Task Handle_WorkOrderNotInInspection_ReturnsFailure()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var technician = Technician.Create(
            "TECH-004",
            "David",
            "Brown",
            "david.brown@test.com",
            "555-0103",
            CertificationLevel.Junior,
            5);

        dbContext.Technicians.Add(technician);

        var workOrder = WorkOrder.Create(
            "WO-TEST-004",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial maintenance note");

        var assignResult = workOrder.AssignTechnician(
            technician,
            "Technician assigned");

        Assert.IsTrue(assignResult.IsSuccess);

        var inProgressResult = workOrder.InProgress(
            "Maintenance work started");

        Assert.IsTrue(inProgressResult.IsSuccess);

        // Deliberately do NOT move the work order to Inspection.

        dbContext.WorkOrders.Add(workOrder);

        await dbContext.SaveChangesAsync();

        var handler = new CompletedWorkOrderCommandHandler(dbContext);

        var command = new CompletedWorkOrderCommand(
            workOrder.Id,
            "Attempted completion",
            4.5m);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsSuccess);

        Assert.AreEqual(
            "Only work orders in inspection can be completed.",
            result.ErrorMessage);
    }

    [TestMethod]
    public async Task Handle_ValidCompletion_PersistsChangesToDatabase()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var technician = Technician.Create(
            "TECH-005",
            "Sarah",
            "Wilson",
            "sarah.wilson@test.com",
            "555-0104",
            CertificationLevel.Junior,
            6);

        dbContext.Technicians.Add(technician);

        var workOrder = WorkOrder.Create(
            "WO-TEST-005",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.High,
            DateTime.UtcNow.AddDays(7),
            "Initial maintenance note");

        var assignResult = workOrder.AssignTechnician(
            technician,
            "Technician assigned");

        Assert.IsTrue(assignResult.IsSuccess);

        var inProgressResult = workOrder.InProgress(
            "Maintenance work started");

        Assert.IsTrue(inProgressResult.IsSuccess);

        var inspectionResult = workOrder.Inspection(
            "Inspection completed");

        Assert.IsTrue(inspectionResult.IsSuccess);

        dbContext.WorkOrders.Add(workOrder);

        await dbContext.SaveChangesAsync();

        var handler = new CompletedWorkOrderCommandHandler(dbContext);

        var command = new CompletedWorkOrderCommand(
            workOrder.Id,
            "Final maintenance completed",
            6.5m);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var savedWorkOrder = await dbContext.WorkOrders
            .FirstAsync(w => w.Id == workOrder.Id);

        Assert.AreEqual(
            WorkOrderStatus.Completed,
            savedWorkOrder.WorkOrderStatus);

        Assert.AreEqual(
            6.5m,
            savedWorkOrder.LaborHours);

        Assert.AreEqual(
            "Final maintenance completed",
            savedWorkOrder.LaborNotes);

        Assert.IsNotNull(
            savedWorkOrder.ActualCompletionDate);
    }
}
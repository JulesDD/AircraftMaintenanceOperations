using AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.IssueInventory;
using AircraftMaintenanceOperations.Application.Interfaces;
using AircraftMaintenanceOperations.Domain.Entities;
using AircraftMaintenanceOperations.Domain.Enums;
using AircraftMaintenanceOperations.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace AircraftMaintenanceOperations.Tests.Application.Features.InventoryTransactions.Commands;

[TestClass]
public class IssueInventoryCommandHandlerTests
{
    private AircraftMaintenanceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AircraftMaintenanceDbContext(options);
    }

    [TestMethod]
    public async Task Handle_WithValidQuantity_ShouldDecreaseStockAndCreateUsageAndTransaction()
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

        var workOrder = CreateInProgressWorkOrder();

        dbContext.InventoryParts.Add(inventoryPart);
        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(performedByUserId);

        var handler = new IssueInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new IssueInventoryCommand(
            inventoryPart.Id,
            workOrder.Id,
            4,
            "Issued for maintenance");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        var updatedInventory = await dbContext.InventoryParts
            .FirstAsync(x => x.Id == inventoryPart.Id);

        var usage = await dbContext.InventoryUsages
            .FirstOrDefaultAsync();

        var transaction = await dbContext.InventoryTransactions
            .FirstOrDefaultAsync(x => x.Id == result.Id);

        Assert.AreEqual(6, updatedInventory.QuantityOnHand);

        Assert.IsNotNull(usage);
        Assert.AreEqual(inventoryPart.Id, usage.InventoryPartId);
        Assert.AreEqual(workOrder.Id, usage.WorkOrderId);
        Assert.AreEqual(4, usage.QuantityUsed);

        Assert.IsNotNull(transaction);
        Assert.AreEqual(
            InventoryTransactionType.Usage,
            transaction.TransactionType);

        Assert.AreEqual(4, transaction.Quantity);
        Assert.AreEqual(10, transaction.QuantityBefore);
        Assert.AreEqual(6, transaction.QuantityAfter);
        Assert.AreEqual(
            performedByUserId,
            transaction.PerformedByUserId);

        Assert.AreEqual(
            workOrder.Id,
            transaction.WorkOrderId);

        Assert.AreEqual(
            "Issued for maintenance",
            transaction.Reason);
    }

    [TestMethod]
    public async Task Handle_WhenQuantityExceedsStock_ShouldThrowAndNotCreateUsageOrTransaction()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var inventoryPart = InventoryPart.Create(
            "BRK-002",
            "Brake Assembly",
            InventoryPartType.Rotable,
            5,
            2,
            "A-02");

        var workOrder = CreateInProgressWorkOrder();

        dbContext.InventoryParts.Add(inventoryPart);
        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(Guid.NewGuid());

        var handler = new IssueInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new IssueInventoryCommand(
            inventoryPart.Id,
            workOrder.Id,
            10,
            "Too much inventory");

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
            await dbContext.InventoryUsages.CountAsync());

        Assert.AreEqual(
            0,
            await dbContext.InventoryTransactions.CountAsync());
    }

    [TestMethod]
    public async Task Handle_WithZeroQuantity_ShouldThrowAndNotCreateUsageOrTransaction()
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

        var workOrder = CreateInProgressWorkOrder();

        dbContext.InventoryParts.Add(inventoryPart);
        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(Guid.NewGuid());

        var handler = new IssueInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new IssueInventoryCommand(
            inventoryPart.Id,
            workOrder.Id,
            0,
            "Invalid issue");

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
            await dbContext.InventoryUsages.CountAsync());

        Assert.AreEqual(
            0,
            await dbContext.InventoryTransactions.CountAsync());
    }

    [TestMethod]
    public async Task Handle_WithNegativeQuantity_ShouldThrowAndNotCreateUsageOrTransaction()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var inventoryPart = InventoryPart.Create(
            "BRK-004",
            "Brake Assembly",
            InventoryPartType.Rotable,
            10,
            5,
            "A-04");

        var workOrder = CreateInProgressWorkOrder();

        dbContext.InventoryParts.Add(inventoryPart);
        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(Guid.NewGuid());

        var handler = new IssueInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new IssueInventoryCommand(
            inventoryPart.Id,
            workOrder.Id,
            -5,
            "Invalid issue");

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
            await dbContext.InventoryUsages.CountAsync());

        Assert.AreEqual(
            0,
            await dbContext.InventoryTransactions.CountAsync());
    }

    [TestMethod]
    public async Task Handle_WithMissingWorkOrder_ShouldThrowAndNotChangeInventory()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var inventoryPart = InventoryPart.Create(
            "BRK-005",
            "Brake Assembly",
            InventoryPartType.Rotable,
            10,
            5,
            "A-05");

        dbContext.InventoryParts.Add(inventoryPart);
        await dbContext.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(Guid.NewGuid());

        var handler = new IssueInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new IssueInventoryCommand(
            inventoryPart.Id,
            Guid.NewGuid(),
            5,
            "Missing work order");

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
            await dbContext.InventoryUsages.CountAsync());

        Assert.AreEqual(
            0,
            await dbContext.InventoryTransactions.CountAsync());
    }

    [TestMethod]
    public async Task Handle_WhenWorkOrderIsNotInProgress_ShouldThrowAndNotChangeInventory()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var inventoryPart = InventoryPart.Create(
            "BRK-006",
            "Brake Assembly",
            InventoryPartType.Rotable,
            10,
            5,
            "A-06");

        var workOrder = CreateWorkOrder();

        dbContext.InventoryParts.Add(inventoryPart);
        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService(Guid.NewGuid());

        var handler = new IssueInventoryCommandHandler(
            dbContext,
            currentUser);

        var command = new IssueInventoryCommand(
            inventoryPart.Id,
            workOrder.Id,
            5,
            "Work order not active");

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
            await dbContext.InventoryUsages.CountAsync());

        Assert.AreEqual(
            0,
            await dbContext.InventoryTransactions.CountAsync());
    }

    private static WorkOrder CreateInProgressWorkOrder()
    {
        var workOrder = CreateWorkOrder();

        var technician = Technician.Create(
            "TECH-001",
            "John",
            "Smith",
            "john.smith@test.com",
            "403-555-0100",
            CertificationLevel.Intermediate,
            5);

        var assignResult = workOrder.AssignTechnician(
            technician,
            "Technician assigned.");

        Assert.IsTrue(assignResult.IsSuccess);

        var progressResult = workOrder.InProgress(
            "Maintenance started.");

        Assert.IsTrue(progressResult.IsSuccess);

        return workOrder;
    }

    private static WorkOrder CreateWorkOrder()
    {
        return WorkOrder.Create(
            "WO-TEST-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Test work order");
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
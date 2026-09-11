namespace AircraftMaintenanceOperations.Tests.Domain.Entities;

[TestClass]
public class InventoryTransactionTests
{
    [TestMethod]
    public void Create_ShouldCreateValidTransaction()
    {
        // Arrange
        var inventoryPartId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var workOrderId = Guid.NewGuid();
        var quantityBefore = 10;
        var quantity = 5;
        var quantityAfter = 15;

        // Act
        var transaction = InventoryTransaction.Create(
            inventoryPartId,
            InventoryTransactionType.Receipt,
            quantity,
            quantityBefore,
            quantityAfter,
            userId,
            workOrderId,
            "Received new stock");

        // Assert
        Assert.IsNotNull(transaction);

        Assert.AreEqual(inventoryPartId, transaction.InventoryPartId);
        Assert.AreEqual(InventoryTransactionType.Receipt, transaction.TransactionType);
        Assert.AreEqual(quantity, transaction.Quantity);
        Assert.AreEqual(quantityBefore, transaction.QuantityBefore);
        Assert.AreEqual(quantityAfter, transaction.QuantityAfter);
        Assert.AreEqual(workOrderId, transaction.WorkOrderId);
        Assert.AreEqual(userId, transaction.PerformedByUserId);
        Assert.AreEqual("Received new stock", transaction.Reason);

        Assert.IsTrue(transaction.TransactionDate != default);
    }

    [TestMethod]
    public void Create_WhenQuantityIsZero_ShouldThrow()
    {
        // Arrange
        var inventoryPartId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            InventoryTransaction.Create(
                inventoryPartId,
                InventoryTransactionType.Receipt,
                0,
                10,
                10,
                userId,
                null,
                "Invalid transaction"));
    }

    [TestMethod]
    public void Create_WhenQuantityIsNegative_ShouldThrow()
    {
        // Arrange
        var inventoryPartId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            InventoryTransaction.Create(
                inventoryPartId,
                InventoryTransactionType.Receipt,
                -5,
                10,
                15,
                userId,
                null,
                "Invalid transaction"));
    }

    [TestMethod]
    public void Create_ShouldPreserveQuantityBeforeAndAfter()
    {
        // Arrange
        var inventoryPartId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var transaction = InventoryTransaction.Create(
            inventoryPartId,
            InventoryTransactionType.Adjustment,
            3,
            10,
            13,
            userId,
            null,
            "Inventory adjustment");

        // Assert
        Assert.AreEqual(10, transaction.QuantityBefore);
        Assert.AreEqual(13, transaction.QuantityAfter);
    }

    [TestMethod]
    public void Create_ShouldPreservePerformedByUserId()
    {
        // Arrange
        var inventoryPartId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var transaction = InventoryTransaction.Create(
            inventoryPartId,
            InventoryTransactionType.Receipt,
            5,
            10,
            15,
            userId,
            null,
            "Received stock");

        // Assert
        Assert.AreEqual(userId, transaction.PerformedByUserId);
    }

    [TestMethod]
    public void Create_ShouldPreserveWorkOrderId()
    {
        // Arrange
        var inventoryPartId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var workOrderId = Guid.NewGuid();

        // Act
        var transaction = InventoryTransaction.Create(
            inventoryPartId,
            InventoryTransactionType.Usage,
            2,
            10,
            8,
            userId,
            workOrderId,
            "Issued for work order");

        // Assert
        Assert.AreEqual(workOrderId, transaction.WorkOrderId);
    }

    [TestMethod]
    public void Create_ShouldAllowNullWorkOrderId()
    {
        // Arrange
        var inventoryPartId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var transaction = InventoryTransaction.Create(
            inventoryPartId,
            InventoryTransactionType.Receipt,
            5,
            10,
            15,
            userId,
            null,
            "Received stock");

        // Assert
        Assert.IsNull(transaction.WorkOrderId);
    }

    [TestMethod]
    public void Create_ShouldPreserveReason()
    {
        // Arrange
        var inventoryPartId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var transaction = InventoryTransaction.Create(
            inventoryPartId,
            InventoryTransactionType.Adjustment,
            2,
            10,
            12,
            userId,
            null,
            "Damaged part discovered");

        // Assert
        Assert.AreEqual("Damaged part discovered", transaction.Reason);
    }
}

namespace AircraftMaintenanceOperations.Tests.Domain.Entities;

[TestClass]
public class WorkOrderTests
{
    [TestMethod]
    public void Conclude_WhenWorkOrderIsInProgress_ChangesStatusToConcluded()
    {

        // Arrange
        var workOrder = WorkOrder.Create(
            "WO-TEST-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial work order notes.");

        var technician = Technician.Create(
            "EMP123",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            CertificationLevel.Junior,
            3);

        var assignmentResult = workOrder.AssignTechnician(technician, "Assigning technician.", Guid.NewGuid());
        Assert.IsTrue(assignmentResult.IsSuccess, assignmentResult.ErrorMessage);

        var startResult = workOrder.InProgress("Starting work order.");
        Assert.IsTrue(startResult.IsSuccess, startResult.ErrorMessage);

        // Act
        var conclusionResult = workOrder.Concluded("Concluding work order.");
        Assert.IsTrue(conclusionResult.IsSuccess, conclusionResult.ErrorMessage);

        Assert.IsTrue(conclusionResult.IsSuccess, conclusionResult.ErrorMessage);
        Assert.AreEqual(WorkOrderStatus.Concluded, workOrder.WorkOrderStatus);
        Assert.AreEqual("Concluding work order.", workOrder.LaborNotes);
    }
}


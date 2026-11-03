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

        Assert.AreEqual(WorkOrderStatus.Concluded, workOrder.WorkOrderStatus);
        Assert.AreEqual("Concluding work order.", workOrder.LaborNotes);
    }

    [TestMethod]
    public void OnHold_WhenWorkOrderIsInProgress_ChangesStatusToOnHold()
    {
        // Arrange
        var workOrder = WorkOrder.Create(
            "WO-TEST-002",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial work order notes.");
        var technician = Technician.Create(
            "EMP456",
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            CertificationLevel.Senior,
            5);


        var assignmentResult = workOrder.AssignTechnician(technician, "Assigning technician.", Guid.NewGuid());
        Assert.IsTrue(assignmentResult.IsSuccess, assignmentResult.ErrorMessage);

        var startResult = workOrder.InProgress("Starting work order.");
        Assert.IsTrue(startResult.IsSuccess, startResult.ErrorMessage);

        // Act
        var holdResult = workOrder.OnHold(HoldReason.Parts, "Waiting for parts to arrive.");

        Assert.IsTrue(holdResult.IsSuccess, holdResult.ErrorMessage);
        Assert.AreEqual(WorkOrderStatus.OnHold, workOrder.WorkOrderStatus);
        Assert.AreEqual(HoldReason.Parts, workOrder.HoldReason);
        Assert.AreEqual("Waiting for parts to arrive.", workOrder.HoldNotes);
        Assert.AreEqual(WorkOrderStatus.InProgress, workOrder.PreviousStatus);
        Assert.IsNotNull(workOrder.HoldDate);
    }

    [TestMethod]
    public void OnHold_WhenWorkOrderIsAlreadyOnHold_ReturnsError()
    {
        // Arrange
        var workOrder = WorkOrder.Create(
            "WO-TEST-003",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial work order notes.");
        var technician = Technician.Create(
            "EMP789",
            "Alice",
            "Johnson",
            "alice.johnson@test.com",
            "555-9012",
            CertificationLevel.Senior,
            7);

        var assignmentResult = workOrder.AssignTechnician(technician, "Assigning technician.", Guid.NewGuid());
        Assert.IsTrue(assignmentResult.IsSuccess, assignmentResult.ErrorMessage);

        var startResult = workOrder.InProgress("Starting work order.");
        Assert.IsTrue(startResult.IsSuccess, startResult.ErrorMessage);

        var holdResult1 = workOrder.OnHold(HoldReason.Clarification, "Waiting for clarification.");
        Assert.IsTrue(holdResult1.IsSuccess, holdResult1.ErrorMessage);

        var holdResult2 = workOrder.OnHold(HoldReason.Revision, "Waiting for revision.");
        Assert.IsFalse(holdResult2.IsSuccess);
        Assert.AreEqual("Work order is already on hold.", holdResult2.ErrorMessage);
    }

    [TestMethod]
    public void OnHold_WhenWorkOrderIsInspection_ReturnsError()
    {
        // Arrange
        var workOrder = WorkOrder.Create(
            "WO-TEST-004",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial work order notes.");
        var technician = Technician.Create(
            "EMP321",
            "Bob",
            "Williams",
            "bob.williams@test.com",
            "555-3456",
            CertificationLevel.Senior,
            7);

        var assignmentResult = workOrder.AssignTechnician(technician, "Assigning technician.", Guid.NewGuid());
        Assert.IsTrue(assignmentResult.IsSuccess, assignmentResult.ErrorMessage);

        var startResult = workOrder.InProgress("Starting work order.");
        Assert.IsTrue(startResult.IsSuccess, startResult.ErrorMessage);

        var concludeResult = workOrder.Concluded("Concluding work order.");
        Assert.IsTrue(concludeResult.IsSuccess, concludeResult.ErrorMessage);

        var inspectionResult = workOrder.Inspection("Moving to inspection.");
        Assert.IsTrue(inspectionResult.IsSuccess, inspectionResult.ErrorMessage);

        var holdResult = workOrder.OnHold(HoldReason.Safety, "Waiting for safety review.");
        Assert.IsFalse(holdResult.IsSuccess);
        Assert.IsTrue(holdResult.ErrorMessage.Contains("Only assigned or in-progress work orders can be put on hold."));
    }

    [TestMethod]
    public void OnHold_WhenWorkOrderIsCompleted_ReturnsError()
    {
        // Arrange
        var workOrder = WorkOrder.Create(
            "WO-TEST-005",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial work order notes.");
        var technician = Technician.Create(
            "EMP654",
            "Charlie",
            "Brown",
            "charlie.brown@test.com",
            "555-6789",
            CertificationLevel.Senior,
            7);

        var assignmentResult = workOrder.AssignTechnician(technician, "Assigning technician.", Guid.NewGuid());
        Assert.IsTrue(assignmentResult.IsSuccess, assignmentResult.ErrorMessage);

        var startResult = workOrder.InProgress("Starting work order.");
        Assert.IsTrue(startResult.IsSuccess, startResult.ErrorMessage);

        var concludeResult = workOrder.Concluded("Concluding work order.");
        Assert.IsTrue(concludeResult.IsSuccess, concludeResult.ErrorMessage);

        var inspectionResult = workOrder.Inspection("Moving to inspection.");
        Assert.IsTrue(inspectionResult.IsSuccess, inspectionResult.ErrorMessage);

        var completeResult = workOrder.Completed("Completing work order.", 6.5m);
        Assert.IsTrue(completeResult.IsSuccess, completeResult.ErrorMessage);
        Assert.AreEqual(WorkOrderStatus.Completed, workOrder.WorkOrderStatus);
        Assert.IsTrue(workOrder.LaborHours != null, "Labor hours should not be null.");

        var holdResult = workOrder.OnHold(HoldReason.Clarification, "Waiting for clarification.");
        Assert.IsFalse(holdResult.IsSuccess);
        Assert.AreEqual("Only assigned or in-progress work orders can be put on hold.", holdResult.ErrorMessage);
    }

    [TestMethod]
    public void OnHold_WhenWorkOrderIsArchived_ReturnsError()
    {
        // Arrange
        var workOrder = WorkOrder.Create(
            "WO-TEST-005",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial work order notes.");
        var technician = Technician.Create(
            "EMP654",
            "Charlie",
            "Brown",
            "charlie.brown@test.com",
            "555-6789",
            CertificationLevel.Senior,
            7);

        var assignmentResult = workOrder.AssignTechnician(technician, "Assigning technician.", Guid.NewGuid());
        Assert.IsTrue(assignmentResult.IsSuccess, assignmentResult.ErrorMessage);

        var startResult = workOrder.InProgress("Starting work order.");
        Assert.IsTrue(startResult.IsSuccess, startResult.ErrorMessage);

        var concludeResult = workOrder.Concluded("Concluding work order.");
        Assert.IsTrue(concludeResult.IsSuccess, concludeResult.ErrorMessage);

        var inspectionResult = workOrder.Inspection("Moving to inspection.");
        Assert.IsTrue(inspectionResult.IsSuccess, inspectionResult.ErrorMessage);

        var completeResult = workOrder.Completed("Completing work order.", 6.5m);
        Assert.IsTrue(completeResult.IsSuccess, completeResult.ErrorMessage);

        var archiveResult = workOrder.ArchiveWorkOrder("Archiving work order.");
        Assert.IsTrue(archiveResult.IsSuccess, archiveResult.ErrorMessage);

        var holdResult = workOrder.OnHold(HoldReason.Clarification, "Waiting for clarification.");
        Assert.IsFalse(holdResult.IsSuccess);
        Assert.AreEqual("Only assigned or in-progress work orders can be put on hold.", holdResult.ErrorMessage);
    }

    [TestMethod]
    public void OnHold_WhenReasonIsNull_ReturnsError()
    {
        // Arrange
        var workOrder = WorkOrder.Create(
            "WO-TEST-006",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial work order notes.");
        var technician = Technician.Create(
            "EMP987",
            "David",
            "Green",
            "david.green@test.com",
            "555-0123",
            CertificationLevel.Senior,
            7);

        var assignmentResult = workOrder.AssignTechnician(technician, "Assigning technician.", Guid.NewGuid());
        Assert.IsTrue(assignmentResult.IsSuccess, assignmentResult.ErrorMessage);

        var startResult = workOrder.InProgress("Starting work order.");
        Assert.IsTrue(startResult.IsSuccess, startResult.ErrorMessage);

        var holdResult = workOrder.OnHold(null, "Waiting for clarification.");
        Assert.IsFalse(holdResult.IsSuccess);
        Assert.AreEqual("A valid hold reason is required.", holdResult.ErrorMessage);
    }

    [TestMethod]
    public void OnHold_WhenNotesAreNull_ReturnsError()
    {
        // Arrange
        var workOrder = WorkOrder.Create(
            "WO-TEST-007",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial work order notes.");
        var technician = Technician.Create(
            "EMP654",
            "Charlie",
            "Brown",
            "charlie.brown@test.com",
            "555-0123",
            CertificationLevel.Senior,
            7);

        var assignmentResult = workOrder.AssignTechnician(technician, "Assigning technician.", Guid.NewGuid());
        Assert.IsTrue(assignmentResult.IsSuccess, assignmentResult.ErrorMessage);

        var startResult = workOrder.InProgress("Starting work order.");
        Assert.IsTrue(startResult.IsSuccess, startResult.ErrorMessage);

        var holdResult = workOrder.OnHold(HoldReason.Clarification, null);
        Assert.IsFalse(holdResult.IsSuccess);
        Assert.AreEqual("Hold notes are required.", holdResult.ErrorMessage);
    }

    [TestMethod]
    public void OnHold_WhenNotesAreWhitespace_ReturnsError()
    {
        // Arrange
        var workOrder = WorkOrder.Create(
            "WO-TEST-007",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial work order notes.");
        var technician = Technician.Create(
            "EMP654",
            "Charlie",
            "Brown",
            "charlie.brown@test.com",
            "555-0123",
            CertificationLevel.Senior,
            7);

        var assignmentResult = workOrder.AssignTechnician(technician, "Assigning technician.", Guid.NewGuid());
        Assert.IsTrue(assignmentResult.IsSuccess, assignmentResult.ErrorMessage);

        var startResult = workOrder.InProgress("Starting work order.");
        Assert.IsTrue(startResult.IsSuccess, startResult.ErrorMessage);

        var holdResult = workOrder.OnHold(HoldReason.Clarification, "  ");
        Assert.IsFalse(holdResult.IsSuccess);
        Assert.AreEqual("Hold notes are required.", holdResult.ErrorMessage);
    }

    [TestMethod]
    public void OnHold_WhenHoldReasonIsInvalid_ReturnsError()
    {
        // Arrange
        var workOrder = WorkOrder.Create(
            "WO-TEST-002",
            Guid.NewGuid(),
            Guid.NewGuid(),
            MaintenancePriority.Medium,
            DateTime.UtcNow.AddDays(7),
            "Initial work order notes.");
        var technician = Technician.Create(
            "EMP456",
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            CertificationLevel.Senior,
            5);

        var assignmentResult = workOrder.AssignTechnician(technician, "Assigning technician.", Guid.NewGuid());
        Assert.IsTrue(assignmentResult.IsSuccess, assignmentResult.ErrorMessage);

        var startResult = workOrder.InProgress("Starting work order.");
        Assert.IsTrue(startResult.IsSuccess, startResult.ErrorMessage);

        // Act
        var invalidReason = (HoldReason)999;
        var holdResult = workOrder.OnHold(invalidReason, "Waiting for clarification.");
        Assert.IsFalse(holdResult.IsSuccess);
        Assert.AreEqual("A valid hold reason is required.", holdResult.ErrorMessage);
    }
}
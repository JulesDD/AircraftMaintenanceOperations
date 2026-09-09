using AircraftMaintenanceOperations.Domain.Entities;
using AircraftMaintenanceOperations.Domain.Enums;

namespace AircraftMaintenanceOperations.Tests.Domain.Entities;

[TestClass]
public class UserTests
{
    [TestMethod]
    public void Create_ShouldCreateActiveUserWithSpecifiedRole()
    {
        // Arrange
        var employeeNumber = "EMP-001";
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@test.com";
        var phoneNumber = "555-1234";
        var role = Role.Technician;

        // Act
        var user = User.Create(
            employeeNumber,
            firstName,
            lastName,
            email,
            phoneNumber,
            role);

        // Assert
        Assert.IsNotNull(user);
        Assert.AreEqual(employeeNumber, user.EmployeeNumber);
        Assert.AreEqual(firstName, user.FirstName);
        Assert.AreEqual(lastName, user.LastName);
        Assert.AreEqual(email, user.Email);
        Assert.AreEqual(phoneNumber, user.PhoneNumber);
        Assert.AreEqual(role, user.Role);
        Assert.AreEqual(EmploymentStatus.Active, user.Status);
        Assert.AreNotEqual(Guid.Empty, user.Id);
    }

    [TestMethod]
    public void ChangeRole_ShouldChangeCurrentRole()
    {
        // Arrange
        var user = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        // Act
        user.ChangeRole(Role.MaintenanceSupervisor);

        // Assert
        Assert.AreEqual(Role.MaintenanceSupervisor, user.Role);
    }

    [TestMethod]
    public void ChangeRole_ShouldPreserveEmployeeInformation()
    {
        // Arrange
        var user = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        // Act
        user.ChangeRole(Role.MaintenanceSupervisor);

        // Assert
        Assert.AreEqual("EMP-001", user.EmployeeNumber);
        Assert.AreEqual("John", user.FirstName);
        Assert.AreEqual("Doe", user.LastName);
        Assert.AreEqual("john.doe@test.com", user.Email);
        Assert.AreEqual("555-1234", user.PhoneNumber);
        Assert.AreEqual(Role.MaintenanceSupervisor, user.Role);
    }

    [TestMethod]
    public void ChangeRole_WhenAlreadyAssignedRole_ShouldThrow()
    {
        // Arrange
        var user = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => user.ChangeRole(Role.Technician));
    }

    [TestMethod]
    public void ChangeRole_WhenRetired_ShouldThrow()
    {
        // Arrange
        var user = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        user.Retire();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => user.ChangeRole(Role.MaintenanceSupervisor));
    }

    [TestMethod]
    public void ChangeRole_WhenArchived_ShouldThrow()
    {
        // Arrange
        var user = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        user.Archive();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => user.ChangeRole(Role.MaintenanceSupervisor));
    }

    [TestMethod]
    public void StartVacation_ShouldChangeStatusToVacation()
    {
        // Arrange
        var user = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        // Act
        user.StartVacation();

        // Assert
        Assert.AreEqual(EmploymentStatus.Vacation, user.Status);
    }

    [TestMethod]
    public void EndVacation_ShouldReturnStatusToActive()
    {
        // Arrange
        var user = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        user.StartVacation();

        // Act
        user.EndVacation();

        // Assert
        Assert.AreEqual(EmploymentStatus.Active, user.Status);
    }

    [TestMethod]
    public void Suspend_ShouldChangeStatusToSuspended()
    {
        // Arrange
        var user = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        // Act
        user.Suspend();

        // Assert
        Assert.AreEqual(EmploymentStatus.Suspended, user.Status);
    }

    [TestMethod]
    public void Reinstate_ShouldReturnSuspendedUserToActive()
    {
        // Arrange
        var user = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        user.Suspend();

        // Act
        user.Reinstate();

        // Assert
        Assert.AreEqual(EmploymentStatus.Active, user.Status);
    }

    [TestMethod]
    public void Retire_ShouldChangeStatusToRetired()
    {
        // Arrange
        var user = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        // Act
        user.Retire();

        // Assert
        Assert.AreEqual(EmploymentStatus.Retired, user.Status);
    }

    [TestMethod]
    public void Archive_ShouldChangeStatusToArchived()
    {
        // Arrange
        var user = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        // Act
        user.Archive();

        // Assert
        Assert.AreEqual(EmploymentStatus.Archived, user.Status);
    }
}

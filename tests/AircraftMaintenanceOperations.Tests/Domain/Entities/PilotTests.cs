namespace AircraftMaintenanceOperations.Tests.Domain.Entities;

[TestClass]
public class PilotTests
{
    [TestMethod]
    public void CreatePilot_ShouldCreateActivePilotWithSpecifiedRole()
    {
        // Arrange
        var employeeNumber = "EMP-001";
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@test.com";
        var phoneNumber = "555-1234";
        var rank = "First Captain";
        var licenseNumber = "5802-484848";

        // Act
        var pilot = Pilot.Create(
            employeeNumber,
            firstName,
            lastName,
            email,
            phoneNumber,
            rank,
            licenseNumber);

        // Assert
        Assert.IsNotNull(pilot);
        Assert.AreEqual(employeeNumber, pilot.EmployeeNumber);
        Assert.AreEqual(firstName, pilot.FirstName);
        Assert.AreEqual(lastName, pilot.LastName);
        Assert.AreEqual(email, pilot.Email);
        Assert.AreEqual(phoneNumber, pilot.PhoneNumber);
        Assert.AreEqual(rank, pilot.Rank);
        Assert.AreEqual(licenseNumber, pilot.LicenseNumber);
        Assert.AreEqual(EmploymentStatus.Active, pilot.Status);
        Assert.AreNotEqual(Guid.Empty, pilot.Id);
    }

    [TestMethod]
    public void UpdateRank_ShouldUpdateRank()
    {
        // Arrange
        var pilot = Pilot.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            "First Captain",
            "5802-484848");

        // Act
        pilot.Update(
            firstName: null,
            lastName: null,
            email: null,
            phoneNumber: null,
            rank: "Senior Captain",
            licenseNumber: null);

        // Assert
        Assert.AreEqual("Senior Captain", pilot.Rank);
    }

    [TestMethod]
    public void UpdateLicenseNumber_ShouldUpdateLicenseNumber()
    {
        // Arrange
        var pilot = Pilot.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            "First Captain",
            "5802-484848");

        // Act
        pilot.Update(
            firstName: null,
            lastName: null,
            email: null,
            phoneNumber: null,
            rank: null,
            licenseNumber: "5802-484849");

        // Assert
        Assert.AreEqual("5802-484849", pilot.LicenseNumber);
    }

    [TestMethod]
    public void ChangeRole_ShouldPreservePilotProfile()
    {
        var pilot = Pilot.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            "First Captain",
            "5802-484848");

        pilot.ChangeRole(Role.MaintenanceSupervisor);

        Assert.AreEqual(Role.MaintenanceSupervisor, pilot.Role);
        Assert.AreEqual("First Captain", pilot.Rank);
        Assert.AreEqual("5802-484848", pilot.LicenseNumber);
    }
}

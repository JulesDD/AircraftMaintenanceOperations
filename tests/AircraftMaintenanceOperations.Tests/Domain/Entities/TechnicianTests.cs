namespace AircraftMaintenanceOperations.Tests.Domain.Entities;

[TestClass]
public class TechnicianTests
{
    [TestMethod]
    public void Create_ShouldCreateActiveTechnicianWithSpecifiedRole()
    {
        // Arrange
        var employeeNumber = "EMP-002";
        var firstName = "Jane";
        var lastName = "Smith";
        var email = "jane.smith@test.com";
        var phoneNumber = "555-5678";
        var certificationLevel = CertificationLevel.Junior;
        var yearsOfExperience = 5;

        // Act
        var technician = Technician.Create(
            employeeNumber,
            firstName,
            lastName,
            email,
            phoneNumber,
            certificationLevel,
            yearsOfExperience);

        // Assert
        Assert.IsNotNull(technician);
        Assert.AreEqual(employeeNumber, technician.EmployeeNumber);
        Assert.AreEqual(firstName, technician.FirstName);
        Assert.AreEqual(lastName, technician.LastName);
        Assert.AreEqual(email, technician.Email);
        Assert.AreEqual(phoneNumber, technician.PhoneNumber);
        Assert.AreEqual(certificationLevel, technician.CertificationLevel);
        Assert.AreEqual(yearsOfExperience, technician.YearsOfExperience);
        Assert.AreEqual(EmploymentStatus.Active, technician.Status);
        Assert.AreNotEqual(Guid.Empty, technician.Id);
    }

    [TestMethod]
    public void UpdateLastName_ShouldUpdateLastName()
    {
        // Arrange
        var technician = Technician.Create(
            "EMP-002",
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            CertificationLevel.Junior,
            5);

        // Act
        technician.Update(
            "Jane",
            "Doe",
            "jane.doe@test.com",
            "555-8765",
            CertificationLevel.Intermediate,
            6);

        // Assert
        Assert.AreEqual("Doe", technician.LastName);
    }

    [TestMethod]
    public void UpdateFirstName_ShouldUpdateFirstName()
    {
        // Arrange
        var technician = Technician.Create(
            "EMP-002",
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            CertificationLevel.Junior,
            5);

        // Act
        technician.Update(
            "John",
            "Smith",
            "john.smith@test.com",
            "555-8765",
            CertificationLevel.Intermediate,
            6);

        // Assert
        Assert.AreEqual("John", technician.FirstName);
    }

    [TestMethod]
    public void UpdateEmail_ShouldUpdateEmail()
    {
        // Arrange
        var technician = Technician.Create(
            "EMP-002",
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            CertificationLevel.Junior,
            5);

        // Act
        technician.Update(
            "Jane",
            "Smith",
            "jane.doe@test.com",
            "555-8765",
            CertificationLevel.Intermediate,
            6);

        // Assert
        Assert.AreEqual("jane.doe@test.com", technician.Email);
    }

    [TestMethod]
    public void UpdatePhoneNumber_ShouldUpdatePhoneNumber()
    {
        // Arrange
        var technician = Technician.Create(
            "EMP-002",
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            CertificationLevel.Junior,
            5);

        // Act
        technician.Update(
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-8765",
            CertificationLevel.Intermediate,
            6);

        // Assert
        Assert.AreEqual("555-8765", technician.PhoneNumber);
    }

    [TestMethod]
    public void UpdateCertificationLevel_ShouldUpdateCertificationLevel()
    {
        // Arrange
        var technician = Technician.Create(
            "EMP-002",
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            CertificationLevel.Junior,
            5);

        // Act
        technician.Update(
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            CertificationLevel.Intermediate,
            5);

        // Assert
        Assert.AreEqual(CertificationLevel.Intermediate, technician.CertificationLevel);
    }

    [TestMethod]
    public void UpdateYearsOfExperience_ShouldUpdateYearsOfExperience()
    {
        // Arrange
        var technician = Technician.Create(
            "EMP-002",
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            CertificationLevel.Junior,
            5);

        // Act
        technician.Update(
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            CertificationLevel.Junior,
            6);

        // Assert
        Assert.AreEqual(6, technician.YearsOfExperience);
    }

    [TestMethod]
    public void ChangeRole_ShouldPreserveTechnicianProfile()
    {
        var technician = Technician.Create(
            "EMP-002",
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            CertificationLevel.Intermediate,
            6);

        technician.ChangeRole(Role.MaintenanceSupervisor);

        Assert.AreEqual(Role.MaintenanceSupervisor, technician.Role);
        Assert.AreEqual(CertificationLevel.Intermediate, technician.CertificationLevel);
        Assert.AreEqual(6, technician.YearsOfExperience);
    }
}

namespace AircraftMaintenanceOperations.Tests.Infrastructure.Persistence;

[TestClass]
public class TechnicianPersistenceTests
{
    [TestMethod]
    public async Task Technician_ShouldPersistSpecializedProfile()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var technicianId = Guid.Empty;

        await using (var dbContext = new AircraftMaintenanceDbContext(options))
        {
            var technician = Technician.Create(
                "TECH-001",
                "Mike",
                "Johnson",
                "mike.johnson@test.com",
                "555-1111",
                CertificationLevel.Junior,
                10);

            technicianId = technician.Id;

            dbContext.Technicians.Add(technician);

            await dbContext.SaveChangesAsync();
        }

        // Act
        await using (var dbContext = new AircraftMaintenanceDbContext(options))
        {
            var savedTechnician = await dbContext.Technicians
                .SingleAsync(x => x.Id == technicianId);

            // Assert
            Assert.AreEqual("TECH-001", savedTechnician.EmployeeNumber);
            Assert.AreEqual("Mike", savedTechnician.FirstName);
            Assert.AreEqual("Johnson", savedTechnician.LastName);
            Assert.AreEqual(Role.Technician, savedTechnician.Role);
            Assert.AreEqual(CertificationLevel.Junior, savedTechnician.CertificationLevel);
            Assert.AreEqual(10, savedTechnician.YearsOfExperience);
            Assert.AreEqual(EmploymentStatus.Active, savedTechnician.Status);
        }
    }

    [TestMethod]
    public async Task Technician_WhenRoleChanges_ShouldPreserveSpecializedProfile()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var technicianId = Guid.Empty;

        await using (var dbContext = new AircraftMaintenanceDbContext(options))
        {
            var technician = Technician.Create(
                "TECH-002",
                "Sarah",
                "Williams",
                "sarah.williams@test.com",
                "555-2222",
                CertificationLevel.Intermediate,
                7);

            technicianId = technician.Id;

            dbContext.Technicians.Add(technician);
            await dbContext.SaveChangesAsync();
        }

        // Act
        await using (var dbContext = new AircraftMaintenanceDbContext(options))
        {
            var technician = await dbContext.Technicians
                .SingleAsync(x => x.Id == technicianId);

            technician.ChangeRole(Role.MaintenanceSupervisor);

            await dbContext.SaveChangesAsync();
        }

        // Assert
        await using (var dbContext = new AircraftMaintenanceDbContext(options))
        {
            var savedTechnician = await dbContext.Technicians
                .SingleAsync(x => x.Id == technicianId);

            Assert.AreEqual(Role.MaintenanceSupervisor, savedTechnician.Role);
            Assert.AreEqual(CertificationLevel.Intermediate, savedTechnician.CertificationLevel);
            Assert.AreEqual(7, savedTechnician.YearsOfExperience);

            Assert.AreEqual("TECH-002", savedTechnician.EmployeeNumber);
            Assert.AreEqual("Sarah", savedTechnician.FirstName);
            Assert.AreEqual("Williams", savedTechnician.LastName);
        }
    }
}

using AircraftMaintenanceOperations.Domain.Entities;
using AircraftMaintenanceOperations.Domain.Enums;
using AircraftMaintenanceOperations.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AircraftMaintenanceOperations.Tests.Infrastructure.Persistence;

[TestClass]
public class PilotPersistenceTests
{
    [TestMethod]
    public async Task Pilot_ShouldPersistSpecializedProfile()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var pilotId = Guid.Empty;

        await using (var dbContext = new AircraftMaintenanceDbContext(options))
        {
            var pilot = Pilot.Create(
                "PILOT-001",
                "David",
                "Anderson",
                "david.anderson@test.com",
                "555-3333",
                "Captain",
                "ATPL-12345");

            pilotId = pilot.Id;

            dbContext.Pilots.Add(pilot);

            await dbContext.SaveChangesAsync();
        }

        // Act & Assert
        await using (var dbContext = new AircraftMaintenanceDbContext(options))
        {
            var savedPilot = await dbContext.Pilots
                .SingleAsync(x => x.Id == pilotId);

            Assert.AreEqual("PILOT-001", savedPilot.EmployeeNumber);
            Assert.AreEqual("David", savedPilot.FirstName);
            Assert.AreEqual("Anderson", savedPilot.LastName);
            Assert.AreEqual(Role.Pilot, savedPilot.Role);
            Assert.AreEqual("Captain", savedPilot.Rank);
            Assert.AreEqual("ATPL-12345", savedPilot.LicenseNumber);
            Assert.AreEqual(EmploymentStatus.Active, savedPilot.Status);
        }
    }

    [TestMethod]
    public async Task Pilot_WhenRoleChanges_ShouldPreserveSpecializedProfile()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var pilotId = Guid.Empty;

        await using (var dbContext = new AircraftMaintenanceDbContext(options))
        {
            var pilot = Pilot.Create(
                "PILOT-002",
                "Lisa",
                "Thompson",
                "lisa.thompson@test.com",
                "555-4444",
                "First Officer",
                "CPL-67890");

            pilotId = pilot.Id;

            dbContext.Pilots.Add(pilot);

            await dbContext.SaveChangesAsync();
        }

        // Act
        await using (var dbContext = new AircraftMaintenanceDbContext(options))
        {
            var pilot = await dbContext.Pilots
                .SingleAsync(x => x.Id == pilotId);

            pilot.ChangeRole(Role.OperationsManager);

            await dbContext.SaveChangesAsync();
        }

        // Assert
        await using (var dbContext = new AircraftMaintenanceDbContext(options))
        {
            var savedPilot = await dbContext.Pilots
                .SingleAsync(x => x.Id == pilotId);

            Assert.AreEqual(Role.OperationsManager, savedPilot.Role);

            // Specialized Pilot information must survive the role change.
            Assert.AreEqual("First Officer", savedPilot.Rank);
            Assert.AreEqual("CPL-67890", savedPilot.LicenseNumber);

            // Base User information must also survive.
            Assert.AreEqual("PILOT-002", savedPilot.EmployeeNumber);
            Assert.AreEqual("Lisa", savedPilot.FirstName);
            Assert.AreEqual("Thompson", savedPilot.LastName);
        }
    }
}
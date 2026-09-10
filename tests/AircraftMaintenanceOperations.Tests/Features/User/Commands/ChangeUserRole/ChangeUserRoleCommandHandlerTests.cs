namespace AircraftMaintenanceOperations.Tests.Application.Features.User.Commands.ChangeUserRole;

[TestClass]
public class ChangeUserRoleCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_ShouldChangeUserRole()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new AircraftMaintenanceDbContext(options);

        // Arrange
        var employeeNumber = "EMP-001";
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@test.com";
        var phoneNumber = "555-1234";
        var role = Role.Technician;


        var user = global::AircraftMaintenanceOperations.Domain.Entities.User.Create(
            employeeNumber,
            firstName,
            lastName,
            email,
            phoneNumber,
            role);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var identityRoleService = new FakeIdentityRoleService();

        var handler = new ChangeUserRoleCommandHandler(
            dbContext,
            identityRoleService);

        var command = new ChangeUserRoleCommand(
            user.Id,
            Role.MaintenanceSupervisor);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.AreEqual(Role.MaintenanceSupervisor, user.Role);
        Assert.AreEqual(Role.Technician, result.PreviousRole);
        Assert.AreEqual(Role.MaintenanceSupervisor, result.NewRole);
        Assert.AreEqual(user.Id, result.UserId);

        Assert.AreEqual(user.Id, identityRoleService.DomainUserId);
        Assert.AreEqual(
            Role.MaintenanceSupervisor,
            identityRoleService.NewRole);

        Assert.IsTrue(identityRoleService.WasCalled);
    }

    private class FakeIdentityRoleService : IIdentityRoleService
    {
        public bool WasCalled { get; private set; }
        public Guid DomainUserId { get; private set; }
        public Role NewRole { get; private set; }

        public Task UpdateUserRoleAsync(
            Guid domainUserId,
            Role newRole,
            CancellationToken cancellationToken)
        {
            WasCalled = true;
            DomainUserId = domainUserId;
            NewRole = newRole;

            return Task.CompletedTask;
        }
    }

    [TestMethod]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrow()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new AircraftMaintenanceDbContext(options);

        var identityRoleService = new FakeIdentityRoleService();

        var handler = new ChangeUserRoleCommandHandler(
            dbContext,
            identityRoleService);

        var command = new ChangeUserRoleCommand(
            Guid.NewGuid(),
            Role.MaintenanceSupervisor);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.IsFalse(identityRoleService.WasCalled);
    }

    [TestMethod]
    public async Task Handle_WhenUserAlreadyHasRole_ShouldThrowAndNotUpdateIdentity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new AircraftMaintenanceDbContext(options);

        var user = global::AircraftMaintenanceOperations.Domain.Entities.User.Create(
            "EMP-002",
            "Jane",
            "Smith",
            "jane.smith@test.com",
            "555-5678",
            Role.Technician);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var identityRoleService = new FakeIdentityRoleService();

        var handler = new ChangeUserRoleCommandHandler(
            dbContext,
            identityRoleService);

        var command = new ChangeUserRoleCommand(
            user.Id,
            Role.Technician);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.IsFalse(identityRoleService.WasCalled);
        Assert.AreEqual(Role.Technician, user.Role);
    }

    [TestMethod]
    public async Task Handle_WhenUserIsArchived_ShouldThrowAndNotUpdateIdentity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new AircraftMaintenanceDbContext(options);

        var user = global::AircraftMaintenanceOperations.Domain.Entities.User.Create(
            "EMP-003",
            "Bob",
            "Jones",
            "bob.jones@test.com",
            "555-9012",
            Role.Technician);

        user.Archive();

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var identityRoleService = new FakeIdentityRoleService();

        var handler = new ChangeUserRoleCommandHandler(
            dbContext,
            identityRoleService);

        var command = new ChangeUserRoleCommand(
            user.Id,
            Role.MaintenanceSupervisor);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.IsFalse(identityRoleService.WasCalled);
        Assert.AreEqual(Role.Technician, user.Role);
    }
}
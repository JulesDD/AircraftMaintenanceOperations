using AircraftMaintenanceOperations.Application.Features.User.Commands.CreateUser;

namespace AircraftMaintenanceOperations.Tests.Application.Features.User.Commands.CreateUser;

[TestClass]
public class CreateUserCommandHandlerTests
{
    private AircraftMaintenanceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AircraftMaintenanceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AircraftMaintenanceDbContext(options);
    }

    [TestMethod]
    public async Task Handle_ShouldCreateAndPersistUser()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var command = new CreateUserCommand(
            "EMP-100",
            "Alice",
            "Johnson",
            "alice.johnson@test.com",
            "403-555-1234",
            Role.Technician);

        var handler = new CreateUserCommandHandler(dbContext);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreNotEqual(Guid.Empty, result.Id);

        var savedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == result.Id);

        Assert.IsNotNull(savedUser);

        Assert.AreEqual("EMP-100", savedUser.EmployeeNumber);
        Assert.AreEqual("Alice", savedUser.FirstName);
        Assert.AreEqual("Johnson", savedUser.LastName);
        Assert.AreEqual("alice.johnson@test.com", savedUser.Email);
        Assert.AreEqual("403-555-1234", savedUser.PhoneNumber);
        Assert.AreEqual(Role.Technician, savedUser.Role);
        Assert.AreEqual(EmploymentStatus.Active, savedUser.Status);
    }
}

namespace AircraftMaintenanceOperations.Tests.Infrastructure.Authentication;

[TestClass]
public class IdentityRoleServiceTests
{
    [TestMethod]
    public async Task UpdateUserRoleAsync_ShouldReplaceIdentityRole()
    {
        // Arrange
        var databaseName = Guid.NewGuid().ToString();

        var services = new ServiceCollection();

        services.AddLogging();

        services.AddDbContext<AircraftMaintenanceDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));

        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AircraftMaintenanceDbContext>();

        await using var serviceProvider = services.BuildServiceProvider();

        var dbContext =
            serviceProvider.GetRequiredService<AircraftMaintenanceDbContext>();

        var userManager =
            serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager =
            serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // Create the Identity roles used by the test.
        var technicianRole =
            await roleManager.CreateAsync(
                new IdentityRole<Guid>(Role.Technician.ToString()));

        Assert.IsTrue(
            technicianRole.Succeeded,
            string.Join(", ", technicianRole.Errors.Select(x => x.Description)));

        var supervisorRole =
            await roleManager.CreateAsync(
                new IdentityRole<Guid>(Role.MaintenanceSupervisor.ToString()));

        Assert.IsTrue(
            supervisorRole.Succeeded,
            string.Join(", ", supervisorRole.Errors.Select(x => x.Description)));

        // Create the domain user.
        var domainUser = User.Create(
            "EMP-001",
            "John",
            "Doe",
            "john.doe@test.com",
            "555-1234",
            Role.Technician);

        dbContext.Users.Add(domainUser);
        await dbContext.SaveChangesAsync();

        // Create the corresponding Identity user.
        var applicationUser = new ApplicationUser
        {
            UserName = "john.doe",
            Email = domainUser.Email,
            DomainUserId = domainUser.Id,
            EmailConfirmed = true
        };

        var createResult =
            await userManager.CreateAsync(applicationUser);

        Assert.IsTrue(
            createResult.Succeeded,
            string.Join(", ", createResult.Errors.Select(x => x.Description)));

        // Give the Identity user the original role.
        var addRoleResult =
            await userManager.AddToRoleAsync(
                applicationUser,
                Role.Technician.ToString());

        Assert.IsTrue(
            addRoleResult.Succeeded,
            string.Join(", ", addRoleResult.Errors.Select(x => x.Description)));

        var identityRoleService =
            new IdentityRoleService(userManager);

        // Act
        await identityRoleService.UpdateUserRoleAsync(
            domainUser.Id,
            Role.MaintenanceSupervisor,
            CancellationToken.None);

        // Assert
        var roles =
            await userManager.GetRolesAsync(applicationUser);

        Assert.AreEqual(1, roles.Count);
        Assert.AreEqual(
            Role.MaintenanceSupervisor.ToString(),
            roles[0]);
    }
}
namespace AircraftMaintenanceOperations.Tests.Infrastructure.Authentication;

[TestClass]
public class AuthorizationAfterRoleChangeTests
{
    [TestMethod]
    public async Task NewToken_AfterRoleChange_ShouldAuthorizeAsMaintenanceSupervisor()
    {
        // Arrange
        var settings = Options.Create(
            new JwtSettings
            {
                Issuer = "AircraftMaintenanceOperations",
                Audience = "AircraftMaintenanceOperations",
                SecretKey = "this-is-a-test-secret-key-that-is-long-enough",
                ExpirationMinutes = 60
            });

        var tokenService = new JwtTokenService(settings);

        var userId = Guid.NewGuid();

        // Simulate a NEW token issued after the role change.
        var tokenResult = tokenService.GenerateToken(userId, "john.doe", new[] { "MaintenanceSupervisor" });

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(tokenResult.AccessToken);

        var identity = new ClaimsIdentity(token.Claims, authenticationType: "TestAuthentication");

        var principal = new ClaimsPrincipal(identity);

        // Configure the same Supervisor policy used by the API.
        var services = new ServiceCollection();

        services.AddLogging();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Supervisor", policy => policy.RequireRole("Admin", "MaintenanceSupervisor"));
        });

        await using var serviceProvider =
            services.BuildServiceProvider();

        var authorizationService =
            serviceProvider.GetRequiredService<IAuthorizationService>();

        // Act
        var result = await authorizationService.AuthorizeAsync(principal, "Supervisor");

        // Assert
        Assert.IsTrue(result.Succeeded);
    }

    [TestMethod]
    public async Task OldTechnicianRole_AfterRoleChange_ShouldNotAuthorizeAsTechnician()
    {
        // Arrange
        var settings = Options.Create(
            new JwtSettings
            {
                Issuer = "AircraftMaintenanceOperations",
                Audience = "AircraftMaintenanceOperations",
                SecretKey = "this-is-a-test-secret-key-that-is-long-enough",
                ExpirationMinutes = 60
            });

        var tokenService = new JwtTokenService(settings);

        var userId = Guid.NewGuid();

        // Simulate a NEW token after the role changed.
        // The old Technician role is no longer present.
        var tokenResult = tokenService.GenerateToken(userId, "john.doe", new[] { "MaintenanceSupervisor" });

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(tokenResult.AccessToken);

        var identity = new ClaimsIdentity(
            token.Claims,
            authenticationType: "TestAuthentication");

        var principal = new ClaimsPrincipal(identity);

        // Configure the same Technician policy used by the API.
        var services = new ServiceCollection();

        services.AddLogging();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Technician", policy => policy.RequireRole("Technician"));
        });

        await using var serviceProvider =
            services.BuildServiceProvider();

        var authorizationService =
            serviceProvider.GetRequiredService<IAuthorizationService>();

        // Act
        var result = await authorizationService.AuthorizeAsync(principal, "Technician");

        // Assert
        Assert.IsFalse(result.Succeeded);
    }
}


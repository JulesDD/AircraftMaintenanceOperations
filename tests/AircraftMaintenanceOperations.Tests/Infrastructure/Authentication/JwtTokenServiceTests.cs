namespace AircraftMaintenanceOperations.Tests.Infrastructure.Authentication;

[TestClass]
public class JwtTokenServiceTests
{
    [TestMethod]
    public void GenerateToken_ShouldIncludeUserIdUsernameAndRoleClaims()
    {
        // Arrange
        var settings = Options.Create(new JwtSettings
        {
            Issuer = "AircraftMaintenanceOperations",
            Audience = "AircraftMaintenanceOperations",
            SecretKey = "this-is-a-test-secret-key-that-is-long-enough",
            ExpirationMinutes = 60
        });

        var service = new JwtTokenService(settings);

        var userId = Guid.NewGuid();
        var username = "john.doe";
        var roles = new[]
        {
            "Technician"
        };

        // Act
        var result = service.GenerateToken(
            userId,
            username,
            roles);

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.IsTrue(result.ExpiresAt > DateTime.UtcNow);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.AccessToken);

        Assert.AreEqual(
            userId.ToString(),
            token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);

        Assert.AreEqual(
            username,
            token.Claims.First(x => x.Type == JwtRegisteredClaimNames.UniqueName).Value);

        Assert.IsTrue(
            token.Claims.Any(x =>
                x.Type == ClaimTypes.Role &&
                x.Value == "Technician"));
    }

    [TestMethod]
    public void GenerateToken_ShouldUseCurrentRole()
    {
        // Arrange
        var settings = Options.Create(new JwtSettings
        {
            Issuer = "AircraftMaintenanceOperations",
            Audience = "AircraftMaintenanceOperations",
            SecretKey = "this-is-a-test-secret-key-that-is-long-enough",
            ExpirationMinutes = 60
        });

        var service = new JwtTokenService(settings);

        var userId = Guid.NewGuid();

        // Act
        var result = service.GenerateToken(
            userId,
            "john.doe",
            new[] { "MaintenanceSupervisor" });

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.AccessToken);

        var roleClaims = token.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList();

        Assert.AreEqual(1, roleClaims.Count);
        Assert.AreEqual(
            "MaintenanceSupervisor",
            roleClaims[0]);

        Assert.IsFalse(
            roleClaims.Contains("Technician"));
    }
}
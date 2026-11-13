namespace AircraftMaintenanceOperations.Tests.Integration;

[TestClass]
public class InventoryAuthorizationTests
{
    private const string Issuer = "AMO.IntegrationTests";
    private const string Audience = "AMO.IntegrationTests";

    [TestMethod]
    public async Task GetInventory_WithoutJwt_Returns401Unauthorized()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/inventory");

        Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task GetInventory_WithTechnicianRole_Returns403Forbidden()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                CreateJwtToken("Technician"));

        var response = await client.GetAsync("/api/inventory");

        Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static string CreateJwtToken(string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiWebApplicationFactory.JwtSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "integration-test-user"),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [TestMethod]
    public async Task GetInventory_WithInventoryClerkRole_Returns200Ok()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                CreateJwtToken("InventoryClerk"));

        var response = await client.GetAsync("/api/inventory");

        Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}
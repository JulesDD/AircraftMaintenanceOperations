namespace AircraftMaintenanceOperations.Tests.Infrastructure.Authentication;

[TestClass]
public class JwtValidationTests
{
    private const string Issuer = "AMO-Test-Issuer";
    private const string Audience = "AMO-Test-Audience";

    private const string SecretKey =
        "this-is-a-test-secret-key-that-is-long-enough-for-hmac";

    private static TokenValidationParameters GetProductionValidationParameters()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = Issuer,
                ["Jwt:Audience"] = Audience,
                ["Jwt:SecretKey"] = SecretKey,
                ["Jwt:ExpirationMinutes"] = "60"
            })
            .Build();

        var services = new ServiceCollection();

        services.AddInfrastructureServices(configuration);

        var provider = services.BuildServiceProvider();

        var options = provider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        return options.TokenValidationParameters;
    }

    private static string GenerateToken(
        string? issuer = null,
        string? audience = null,
        string? signingKey = null,
        DateTime? expires = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey ?? SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer ?? Issuer,
            audience: audience ?? Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, "test-user")
            ],
            expires: expires ?? DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static void AssertTokenRejected(
        string token,
        TokenValidationParameters parameters)
    {
        try
        {
            new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
            Assert.Fail("Expected the JWT validation to reject the token.");
        }
        catch (SecurityTokenException)
        {
            // Expected: the token failed validation.
        }
    }
    [TestMethod]
    public void ValidateToken_ValidToken_IsAccepted()
    {
        var parameters = GetProductionValidationParameters();
        var token = GenerateToken();

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
        Assert.IsNotNull(principal);
        Assert.AreEqual("test-user", principal.FindFirst(ClaimTypes.Name)?.Value);
    }

    [TestMethod]
    public void ValidateToken_InvalidSigningKey_IsRejected()
    {
        var parameters = GetProductionValidationParameters();
        var token = GenerateToken(signingKey: "another-test-signing-key-that-is-different");
        AssertTokenRejected(token, parameters);
    }

    [TestMethod]
    public void ValidateToken_InvalidIssuer_IsRejected()
    {
        var parameters = GetProductionValidationParameters();
        var token = GenerateToken(issuer: "Incorrect-Issuer");
        AssertTokenRejected(token, parameters);
    }

    [TestMethod]
    public void ValidateToken_InvalidAudience_IsRejected()
    {
        var parameters = GetProductionValidationParameters();
        var token = GenerateToken(audience: "Incorrect-Audience");
        AssertTokenRejected(token, parameters);
    }

    [TestMethod]
    public void ValidateToken_ExpiredToken_IsRejected()
    {
        var parameters = GetProductionValidationParameters();
        var token = GenerateToken(expires: DateTime.UtcNow.AddMinutes(-2));
        AssertTokenRejected(token, parameters);
    }
}


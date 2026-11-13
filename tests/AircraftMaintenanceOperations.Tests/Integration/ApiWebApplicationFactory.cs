namespace AircraftMaintenanceOperations.Tests.Integration;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestJwtKey = "IntegrationTestSigningKey_MustBeAtLeast32BytesLong_123456";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Jwt:Issuer"] = "AMO.IntegrationTests",
                    ["Jwt:Audience"] = "AMO.IntegrationTests",
                    ["Jwt:SecretKey"] = TestJwtKey,
                    ["Jwt:ExpirationMinutes"] = "30"
                });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AircraftMaintenanceDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AircraftMaintenanceDbContext>>();

            services.AddDbContext<AircraftMaintenanceDbContext>(options =>
            {
                options.UseInMemoryDatabase("AMO.IntegrationTests");
            });
        });
    }

    public static string JwtSigningKey => TestJwtKey;
}
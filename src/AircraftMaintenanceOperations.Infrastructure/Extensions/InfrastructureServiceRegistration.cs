namespace AircraftMaintenanceOperations.Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AircraftMaintenanceDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("AircraftMaintenanceConnection"));
        });

        services.AddScoped<IAircraftMaintenanceDbContext>(provider => provider.GetRequiredService<AircraftMaintenanceDbContext>());

        services.AddScoped<INumberGenerator, NumberGeneratorService>();

        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AircraftMaintenanceDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
            };
        });

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<IAuthenticationService, IdentityAuthenticationService>();

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();


        return services;
    }
}
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

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 12;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 4;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AircraftMaintenanceDbContext>()
            .AddDefaultTokenProviders();

        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection("Jwt"))
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.Issuer),
                "JWT issuer must be configured.")
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.Audience),
                "JWT audience must be configured.")
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.SecretKey)
                && Encoding.UTF8.GetByteCount(settings.SecretKey) >= 32,
                "JWT signing key must be configured and contain at least 32 UTF-8 bytes.")
            .Validate(settings => settings.ExpirationMinutes is > 0 and <= 1440,
                "JWT expiration must be between 1 and 1440 minutes.")
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtSettings>>((options, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<IAuthenticationService, IdentityAuthenticationService>();

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddSingleton<IEventPublisher, KafkaEventPublisher>();

        return services;
    }
}

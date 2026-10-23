var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AircraftMaintenanceConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'AircraftMaintenanceConnection' was not configured.");
}

builder.Services.AddDbContext<AircraftMaintenanceDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IAircraftMaintenanceDbContext>(provider => provider.GetRequiredService<AircraftMaintenanceDbContext>());
builder.Services.AddHostedService<WorkOrderNotificationConsumer>();
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

var host = builder.Build();
host.Run();
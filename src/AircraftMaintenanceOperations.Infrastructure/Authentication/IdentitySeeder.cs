namespace AircraftMaintenanceOperations.Infrastructure.Authentication;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = new[]
        {
            "Admin",
            "MaintenanceSupervisor",
            "Technician",
            "InventoryClerk",
            "Pilot",
            "OperationsManager"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    public static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, AircraftMaintenanceDbContext dbContext)
    {

        var existingUser = await userManager.FindByNameAsync("technician");
        if (existingUser is not null) return;

        var technician = Technician.Create(
            "TECH-001",
            "Test",
            "Technician",
            "technician@amo.local",
            "555-0100",
            CertificationLevel.Junior,
            5);
        dbContext.Technicians.Add(technician);
        await dbContext.SaveChangesAsync();

        var applicationUser = new ApplicationUser
        {
            UserName = "technician",
            Email = "technician@amo.local",
            DomainUserId = technician.Id,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(applicationUser, "Technician123!");
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Failed to create development technician: {errors}");
        }

        var roleResult = await userManager.AddToRoleAsync(applicationUser, "Technician");
        if (!roleResult.Succeeded)
        {
            var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Failed to assign Technician role: {errors}");
        }
    }
}

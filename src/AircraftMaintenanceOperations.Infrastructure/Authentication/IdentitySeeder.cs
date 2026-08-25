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

    private static async Task<User> GetOrCreateUserAsync(AircraftMaintenanceDbContext dbContext, string employeeNumber, string firstName,
    string lastName, string email, string phoneNumber, Role role)
    {
        var existingUser = await dbContext.Users.SingleOrDefaultAsync(u => u.EmployeeNumber == employeeNumber);
        if (existingUser is not null)
            return existingUser;

        var user = User.Create(
            employeeNumber,
            firstName,
            lastName,
            email,
            phoneNumber,
            role);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    private static async Task<Technician> GetOrCreateTechnicianAsync(AircraftMaintenanceDbContext dbContext)
    {
        var existingTechnician = await dbContext.Users.OfType<Technician>().SingleOrDefaultAsync(t => t.EmployeeNumber == "TECH-001");
        if (existingTechnician is not null) return existingTechnician;

        var technician = Technician.Create(
            "TECH-001",
            "Test",
            "Technician",
            "technician@amo.local",
            "555-0010",
            CertificationLevel.Junior,
            5);

        dbContext.Users.Add(technician);
        await dbContext.SaveChangesAsync();

        return technician;
    }

    private static async Task<Pilot> GetOrCreatePilotAsync(AircraftMaintenanceDbContext dbContext)
    {
        var existingPilot = await dbContext.Users.OfType<Pilot>().SingleOrDefaultAsync(p => p.EmployeeNumber == "PLT-001");
        if (existingPilot is not null) return existingPilot;

        var pilot = Pilot.Create(
            "PLT-001",
            "Aviation",
            "Pilot",
            "IamPilot@amo.local",
            "555-0101",
            "Captain",
            "CPL-10004");

        dbContext.Users.Add(pilot);
        await dbContext.SaveChangesAsync();

        return pilot;
    }

    private static async Task SeedApplicationUserAsync(UserManager<ApplicationUser> userManager, User domainUser, string username, string password)
    {
        var existingUser = await userManager.FindByNameAsync(username);
        if (existingUser is not null)
            return;

        var applicationUser = new ApplicationUser
        {
            UserName = username,
            Email = domainUser.Email,
            DomainUserId = domainUser.Id,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(applicationUser, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create development user '{username}': {errors}");
        }

        var roleResult = await userManager.AddToRoleAsync(applicationUser, domainUser.Role.ToString());

        if (!roleResult.Succeeded)
        {
            var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to assign role '{domainUser.Role}' to '{username}': {errors}");
        }
    }

    public static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, AircraftMaintenanceDbContext dbContext)
    {
        var admin = await GetOrCreateUserAsync(
            dbContext,
            "AD-001",
            "Admin",
            "User",
            "Admin@amo.local",
            "555-0001",
            Role.Admin);

        var supervisor = await GetOrCreateUserAsync(
            dbContext,
            "SUP-001",
            "Super",
            "Supervisor",
            "Supervisor@amo.local",
            "555-0011",
            Role.MaintenanceSupervisor);

        var technician = await GetOrCreateTechnicianAsync(dbContext);

        var inventoryClerk = await GetOrCreateUserAsync(
            dbContext,
            "INV-001",
            "Inventory",
            "Clerk",
            "InvClerk@amo.local",
            "555-0100",
            Role.InventoryClerk);

        var pilot = await GetOrCreatePilotAsync(dbContext);

        var operationsManager = await GetOrCreateUserAsync(
            dbContext,
            "OMG-001",
            "Operation",
            "Manager",
            "OpManager@amo.local",
            "555-0110",
            Role.OperationsManager);

        await SeedApplicationUserAsync(
            userManager,
            admin,
            "admin",
            "Admin123!");

        await SeedApplicationUserAsync(
            userManager,
            supervisor,
            "supervisor",
            "Supervisor123!");

        await SeedApplicationUserAsync(
            userManager,
            technician,
            "technician",
            "Technician123!");

        await SeedApplicationUserAsync(
            userManager,
            inventoryClerk,
            "inventoryclerk",
            "Inventory123!");

        await SeedApplicationUserAsync(
            userManager,
            pilot,
            "pilot",
            "Pilot123!");

        await SeedApplicationUserAsync(
            userManager,
            operationsManager,
            "operationsmanager",
            "Operations123!");
    }
}
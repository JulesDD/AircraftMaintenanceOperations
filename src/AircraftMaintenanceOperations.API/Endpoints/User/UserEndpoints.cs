namespace AircraftMaintenanceOperations.API.Endpoints.User;

public class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users").RequireAuthorization("Supervisor");

        group.MapPost("/", async (CreateUserCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/{result.Id}", result);
        })
            .WithName("CreateUser")
            .Produces<CreateUserCommandResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Created User.")
            .WithDescription("Create User.");

        group.MapPut("/{UserId:guid}/role", async (Guid userId, ChangeUserRoleCommand command, ISender sender) =>
        {
            var update = command with { UserId = userId };
            var result = await sender.Send(update);
            return Results.Ok(result);
        })
            .WithName("ChangeUserRole")
            .Produces<ChangeUserRoleCommandResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Updated User Role.")
            .WithDescription("Update User Role.");
    }
}

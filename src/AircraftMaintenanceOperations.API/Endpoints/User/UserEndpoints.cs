using AircraftMaintenanceOperations.Application.Features.User.Commands.ChangeUserRole;
using AircraftMaintenanceOperations.Application.Features.User.Commands.CreateUser;

namespace AircraftMaintenanceOperations.API.Endpoints.User;


public record ChangeUserRoleRequest(Role NewRole);
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

        group.MapPut("/{UserId:guid}/role", async (Guid userId, ChangeUserRoleRequest request, ISender sender) =>
        {
            var command = new ChangeUserRoleCommand(userId, request.NewRole);

            var result = await sender.Send(command);
            return Results.Ok(result);
        })
            .WithName("ChangeUserRole")
            .Produces<ChangeUserRoleCommandResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Updated User Role.")
            .WithDescription("Update User Role.");
    }
}

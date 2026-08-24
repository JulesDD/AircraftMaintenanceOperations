using AircraftMaintenanceOperations.Application.Features.User.Commands.CreateUser;

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
    }
}

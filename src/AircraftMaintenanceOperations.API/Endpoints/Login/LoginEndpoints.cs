namespace AircraftMaintenanceOperations.API.Endpoints.Login;

public class LoginEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async(CreateLoginCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
            .WithName("CreateLogin")
            .Produces<CreateLoginCommandResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Login.")
            .WithDescription("Authenticates a user and returns a JWT access token.");
    }
}

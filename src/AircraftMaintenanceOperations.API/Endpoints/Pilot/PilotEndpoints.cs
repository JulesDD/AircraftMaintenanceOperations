namespace AircraftMaintenanceOperations.API.Endpoints.Pilot;

public record CreatePilotResponse(Guid Id);
public record GetPilotByIdResponse(PilotDto Pilot);
public record UpdatePilotCommandResult(bool IsSuccess);



public class PilotEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pilot").WithTags("Pilot");

        group.MapPost("/", async(CreatePilotCommand command, ISender sender) => 
        {
            var result = await sender.Send(command);

            return Results.Created($"/{result.Id}", new CreatePilotResponse(result.Id));
        })
            .WithName("CreatePilot")
            .Produces<CreatePilotResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Created a new Pilot.")
            .WithDescription("Create Pilot");

        group.MapGet("/", async(ISender sender) =>
        {
            var query = new GetPilotQuery();
            var result = await sender.Send(query);
            return Results.Ok(new GetPilotResult(result.Pilots));
        })
            .WithName("GetPilot")
            .Produces<GetPilotResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Gets a list of Pilots.")
            .WithDescription("Get Pilots");

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var query = new GetPilotByIdQuery(id);
            var result = await sender.Send(query);
            var response = result.Adapt<GetPilotByIdResponse>();
            return Results.Ok(response);
        })
            .WithName("GetPilotById")
            .Produces<GetPilotByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Gets pilot by Id.")
            .WithDescription("Get pilot by Id.");

        group.MapPatch("/{id:guid}/archive", async (Guid id, ISender sender) =>
        {
            var command = new ArchivePilotCommand(id, true);
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
            .WithName("ArchivePilot")
            .Produces<ArchivePilotResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Archive Pilot.")
            .WithDescription("Archive Pilot");

        group.MapPatch("/{id:guid}", async (Guid id, UpdatePilotCommand command, ISender sender) =>
        {

            var update = command with { Id = id };
            
            return Results.Ok(await sender.Send(update));
        })
            .WithName("UpdatePilot")
            .Produces<UpdatePilotCommandResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Pilot.")
            .WithDescription("Update Pilot");
    }
}

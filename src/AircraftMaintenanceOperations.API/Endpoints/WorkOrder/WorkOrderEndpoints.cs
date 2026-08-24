namespace AircraftMaintenanceOperations.API.Endpoints.WorkOrder;

public class WorkOrderEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/workorders").WithTags("WorkOrders");

        group.MapPost("/", async (CreateWorkOrderCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/api/workorders/{result.Id}", result);
        })
            .WithName("CreateWorkOrder")
            .Produces<CreatedWorkOrderResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new work order.")
            .WithDescription("Create Work Order");

        group.MapGet("/", async(ISender sender) =>
        {
            var query = new GetWorkOrderQuery();
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
            .WithName("GetAllWorkOrders")
            .RequireAuthorization("Supervisor")
            .Produces<List<WorkOrderDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Retrieves all work orders.")
            .WithDescription("Get All Work Orders");

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var query = new GetWorkOrderQueryByIdQuery(id);
            var result = await sender.Send(query);
            return Results.Ok(result.WorkOrder);
        })
            .WithName("GetWorkOrderById")
            .RequireAuthorization("WorkOrderAccess")
            .Produces<WorkOrderDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Retrieves a work order by its ID.")
            .WithDescription("Get Work Order By ID");

        group.MapPatch("/{id:guid}", async (Guid id, UpdateWorkOrderCommand request, ISender sender) =>
        {
            var command = new UpdateWorkOrderCommand(
                id,
                request.Title,
                request.Description,
                request.WorkOrderPriority,
                request.EstimatedCompletionDate);
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
            .WithName("UpdateWorkOrder")
            .Produces<UpdateWorkOrderResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Updates an existing work order.")
            .WithDescription("Update Work Order");

        group.MapPatch("/{id:guid}/archive", async (Guid id, ISender sender) =>
        {
            var command = new ArchiveWorkOrderCommand(id, "");
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
            .WithName("ArchiveWorkOrder")
            .Produces<ArchiveWorkOrderResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Archives an existing work order.")
            .WithDescription("Archive Work Order");

        group.MapPatch("/{id:guid}/assign-technician", async (Guid id, AssignTechnicianCommand request, ISender sender) =>
        {
            var command = new AssignTechnicianCommand(id, request.TechnicianId);
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
            .WithName("AssignTechnicianToWorkOrder")
            .Produces<AssignTechnicianResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Assigns a technician to a work order.")
            .WithDescription("Assign Technician to Work Order");
    }
}

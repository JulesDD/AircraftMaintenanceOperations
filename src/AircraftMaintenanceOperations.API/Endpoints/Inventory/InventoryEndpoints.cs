using AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.AdjustInventory;
using AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Queries.GetInventory;
using AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Queries.GetInventoryById;
using AircraftMaintenanceOperations.Domain.Entities;

namespace AircraftMaintenanceOperations.API.Endpoints.ReceiveInventory;

public class InventoryEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventory").WithTags("Inventory").RequireAuthorization("InventoryManagement");

        group.MapPost("/", async (CreateInventoryCommand command, ISender sender) => {
            var result = await sender.Send(command);
            return Results.Created($"/api/inventory/{result.Id}", result);
        })
            .WithName("CreateInventory")
            .Produces<CreateInventoryCommandResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Create Inventory.")
            .WithDescription("Create a new inventory part.");

        group.MapPost("/{inventoryPartId}/receive", async(Guid inventoryPartId, ReceiveInventoryCommand command, ISender sender) =>
        {
            var rInventory = new ReceiveInventoryCommand(inventoryPartId, command.Quantity, command.Reason);
            var result = await sender.Send(rInventory);
            return Results.Created($"/api/inventory/{inventoryPartId}/transactions/{result.Id}", result);
        })
            .WithName("ReceiveInventory")
            .Produces<ReceiveInventoryCommandResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Receieve Inventory.")
            .WithDescription("Receive stock for an inventory part and record the inventory transaction.");

        group.MapPost("/{inventoryPartId}/issue", async(Guid inventoryPartId, Guid workOrderId, IssueInventoryCommand command, ISender sender) =>
        {
            var iInventory = new IssueInventoryCommand(
                inventoryPartId,
                workOrderId,
                command.Quantity,
                command.Reason);

            var result = await sender.Send(iInventory);
            return Results.Created($"/api/inventory/{inventoryPartId}/transactions/{result.Id}", result);
        })
        .WithName("IssueInventory")
        .Produces<IssueInventoryCommandResult>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Issue Inventory.")
        .WithDescription("Issue stock for an inventory part and record the inventory transaction.");

        group.MapPost("/{inventoryPartId}/adjust", async (Guid inventoryPartId, AdjustInventoryCommand command, ISender sender) => {
            
            var aInventory = new AdjustInventoryCommand(
                inventoryPartId,
                command.Quantity, 
                command.Notes);

            var result = await sender.Send(aInventory);
            return Results.Created($"/api/inventory/{inventoryPartId}/transactions/{result.Id}", result);
        })
            .WithName("AdjustInventory")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Adjust Inventory")
            .WithDescription("Adjust Inventory");

        group.MapGet("/", async (ISender sender) =>
        {
            var query = new GetInventoryQuery();
            var result = await sender.Send(query);
            return Results.Ok(new GetInventoryQueryResult(result.Items));
        })
            .WithName("GetInventory")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get All Inventory.")
            .WithDescription("Retrieve all inventory parts.");

        group.MapGet("/{inventoryPartId:guid}", async (Guid id, ISender sender) =>
        {
            var query = new GetInventoryByIdQuery(id);
            var result = await sender.Send(query);
            if (result.Item is null) return Results.NotFound();

            return Results.Ok(result.Item);
        })
            .WithName("GetInventoryPart")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get specific inventory part.")
            .WithDescription("Retrieve a specific inventory part.");

        //group.MapGet("/{inventoryPartId}/transactions", async () => {
        //})
        //    .WithName("GetInventoryTransactions")
        //    .Produces(StatusCodes.Status200OK)
        //    .ProducesProblem(StatusCodes.Status404NotFound)
        //    .WithSummary("Get inventory transactions.")
        //    .WithDescription("Retrieve the transaction history for an inventory part.");
    }
}

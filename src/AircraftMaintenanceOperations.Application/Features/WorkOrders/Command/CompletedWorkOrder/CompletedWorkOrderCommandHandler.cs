namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.CompletedWorkOrder;

public class CompletedWorkOrderCommandHandler(IAircraftMaintenanceDbContext DbContext) : IRequestHandler<CompletedWorkOrderCommand, CompletedWorkOrderResult>
{
    public async Task<CompletedWorkOrderResult> Handle(CompletedWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await DbContext.WorkOrders.FirstOrDefaultAsync(w => w.Id == request.WorkOrderId, cancellationToken);
        if(workOrder is null) return new CompletedWorkOrderResult(false, "Unable to find work order");
        
        var result = workOrder.Completed(request.LaborNotes, request.LaborHours);
        if(!result.IsSuccess) return new CompletedWorkOrderResult(false, result.ErrorMessage);

        await DbContext.SaveChangesAsync(cancellationToken);
        return new CompletedWorkOrderResult(true);
    }
}

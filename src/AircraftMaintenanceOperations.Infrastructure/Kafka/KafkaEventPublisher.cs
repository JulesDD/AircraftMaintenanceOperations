namespace AircraftMaintenanceOperations.Infrastructure.Kafka;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventPublisher()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken)
    {
        var eventJson = JsonSerializer.Serialize(new
        {
            EventType = @event!.GetType().Name,
            Payload = @event
        });

        var message = new Message<string, string>
        {
            Key = GetEventKey(@event),
            Value = eventJson
        };

        await _producer.ProduceAsync(GetTopic(@event), message, cancellationToken);
    }

    private static string GetTopic<T>(T @event)
    {
        return @event switch
        {
            WorkOrderConcludedEvent => "work-order-events",
            WorkOrderCompletedEvent => "work-order-events",
            WorkOrderCreatedEvent => "work-order-events",
            TechnicianAssignedToWorkOrderEvent => "work-order-events",
            WorkOrderInProgressEvent => "work-order-events",
            WorkOrderSentToInspectionEvent => "work-order-events",
            InventoryIssuedForWorkOrderEvent => "inventory-events",
            InventoryAdjustedEvent => "inventory-events",
            InventoryReceivedEvent => "inventory-events",
            _ => throw new InvalidOperationException($"No Kafka topic configured for event type {typeof(T).Name}.")
        };
    }

    private static string GetEventKey<T>(T @event)
    {
        return @event switch
        {
            WorkOrderCompletedEvent workOrderEvent =>
                workOrderEvent.WorkOrderId.ToString(),

            WorkOrderCreatedEvent workOrderEvent =>
                workOrderEvent.WorkOrderId.ToString(),
            
            TechnicianAssignedToWorkOrderEvent workOrderEvnt =>
                workOrderEvnt.WorkOrderId.ToString(),

            WorkOrderInProgressEvent workOrderInProgressEvent =>
                workOrderInProgressEvent.WorkOrderId.ToString(),

            WorkOrderSentToInspectionEvent workOrderSentToInspectionEvent =>
                workOrderSentToInspectionEvent.WorkOrderId.ToString(),

            InventoryIssuedForWorkOrderEvent inventoryIssuedEvent =>
                inventoryIssuedEvent.WorkOrderId.ToString(),

            InventoryAdjustedEvent inventoryAdjustedEvent =>
                inventoryAdjustedEvent.InventoryPartId.ToString(),

            InventoryReceivedEvent inventoryReceivedEvent =>
                inventoryReceivedEvent.InventoryPartId.ToString(),

            WorkOrderConcludedEvent workOrderConcludedEvent =>
                workOrderConcludedEvent.WorkOrderId.ToString(),

            _ => throw new InvalidOperationException(
                $"No Kafka key configured for event type {typeof(T).Name}.")
        };
    }
}

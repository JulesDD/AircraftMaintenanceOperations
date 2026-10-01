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
        var eventJson = JsonSerializer.Serialize(@event);

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
            WorkOrderCompletedEvent => "work-order-events",
            WorkOrderCreatedEvent => "work-order-events",
            _ => throw new InvalidOperationException($"No Kafka topic configured for event type {typeof(T).Name}.")
        };
    }

    private static string GetEventKey<T>(T @event)
    {
        Console.WriteLine($"Generic type: {typeof(T).AssemblyQualifiedName}");
        Console.WriteLine($"Runtime type: {@event?.GetType().AssemblyQualifiedName}");

        Console.WriteLine(
            $"Expected type: {typeof(WorkOrderCreatedEvent).AssemblyQualifiedName}");

        return @event switch
        {
            WorkOrderCompletedEvent workOrderEvent =>
                workOrderEvent.WorkOrderId.ToString(),

            WorkOrderCreatedEvent workOrderEvent =>
                workOrderEvent.WorkOrderId.ToString(),

            _ => throw new InvalidOperationException(
                $"No Kafka key configured for event type {typeof(T).Name}.")
        };
    }
}

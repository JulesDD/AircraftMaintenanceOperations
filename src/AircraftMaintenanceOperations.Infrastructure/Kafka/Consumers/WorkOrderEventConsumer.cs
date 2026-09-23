namespace AircraftMaintenanceOperations.Infrastructure.Kafka.Consumers;

public class WorkOrderEventConsumer
{
    private readonly ConsumerConfig _config;
    public WorkOrderEventConsumer()
    {
        _config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "maintenance-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    public void Start()
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .Build();

        consumer.Subscribe("work-order-events");

        Console.WriteLine("Consumer started. Waiting for messages...");

        while (true)
        {
            var result = consumer.Consume();

            Console.WriteLine($"Topic: {result.Topic} | " + $"Partition: {result.Partition} | " + $"Offset: {result.Offset}");
            Console.WriteLine($"Key: {result.Message.Key}");
            Console.WriteLine($"Value: {result.Message.Value}");
            Console.WriteLine("--------------------------------");
        }
    }
}

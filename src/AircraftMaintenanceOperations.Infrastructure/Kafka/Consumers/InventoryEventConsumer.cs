namespace AircraftMaintenanceOperations.Infrastructure.Kafka.Consumers;

public class InventoryEventConsumer
{
    private readonly ConsumerConfig _config;

    public InventoryEventConsumer()
    {
        _config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "inventory-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    public void Start(CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetErrorHandler((_, error) =>
            {
                Console.Error.WriteLine(
                    $"Kafka error: {error.Code} - {error.Reason} " +
                    $"(Fatal: {error.IsFatal})");
            })
            .Build();

        consumer.Subscribe("inventory-events");

        Console.WriteLine("Inventory consumer started. Waiting for messages...");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);

                    Console.WriteLine($"Topic: {result.Topic} | " + $"Partition: {result.Partition} | " + $"Offset: {result.Offset}");
                    Console.WriteLine($"Key: {result.Message.Key}");
                    Console.WriteLine($"Value: {result.Message.Value}");
                    Console.WriteLine("--------------------------------");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException ex)
                {
                    Console.Error.WriteLine($"Failed to consume message: {ex.Error.Code} - {ex.Error.Reason}");

                    if (ex.Error.IsFatal)
                    {
                        throw;
                    }
                }
            }
        }
        finally
        {
            consumer.Close();
            Console.WriteLine("Inventory consumer shut down cleanly.");
        }
    }
}
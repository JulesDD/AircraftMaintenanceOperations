namespace AircraftMaintenanceOperations.Infrastructure.Kafka;

public class KafkaEventEnvelope
{
    public string EventType { get; set; } = string.Empty;
    public System.Text.Json.JsonElement Payload { get; set; }
}
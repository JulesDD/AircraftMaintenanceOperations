using AircraftMaintenanceOperations.Infrastructure.Kafka.Consumers;

var consumer = new WorkOrderEventConsumer();

consumer.Start();

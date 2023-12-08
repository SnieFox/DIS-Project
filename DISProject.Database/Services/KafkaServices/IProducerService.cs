namespace DISProject.Database.Services.KafkaServices;

public interface IProducerService
{
    Task ProduceAsync(string topic, string message);
}
using Confluent.Kafka;
using System.Text.Json;
using ParkingProject.Models;

namespace ParkingProject.Services
{
    public class KafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic = "api-access-logs";

        public KafkaProducerService(IConfiguration configuration)
        {
            // Holt die Config aus appsettings.json oder Environment Variables
            var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "kafka:29092";

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };

            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public async Task LogAccessAsync(ApiAccessLog logEntry)
        {
            try
            {
                // Objekt zu JSON serialisieren
                string message = JsonSerializer.Serialize(logEntry);

                // Asynchron senden (Feuer und vergiss für Logging)
                await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
            }
            catch (Exception ex)
            {
                // Hier sollten wir aufpassen: Wenn Kafka down ist, soll die API nicht abstürzen.
                // Wir schreiben es zur Not in die Konsole.
                Console.WriteLine($"KAFKA ERROR: {ex.Message}");
            }
        }
    }
}
using Confluent.Kafka;
using System.Text.Json;

public class AlertConsumerService : BackgroundService
{
    private readonly string _topic = "parking-occupancy-changed";
    private readonly ILogger<AlertConsumerService> _logger;
    private readonly IConsumer<Null, string> _consumer; 

    public AlertConsumerService(ILogger<AlertConsumerService> logger)
    {
        _logger = logger;
        var config = new ConsumerConfig
        {
            BootstrapServers = "kafka:29092", 
            GroupId = "alert-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        
        _consumer = new ConsumerBuilder<Null, string>(config).Build(); 
    }    

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            _consumer.Subscribe(_topic);
            _logger.LogInformation($"Kafka Alert Consumer gestartet auf Topic: {_topic}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    var data = JsonSerializer.Deserialize<OccupancyChangeEvent>(result.Message.Value);

                    if (data != null && data.TotalSpots > 0)
                    {
                        // Wenn weniger als 10% frei sind gibt's eine Warnung
                        double ratio = (double)data.FreeSpots / data.TotalSpots;
                        
                        if (ratio < 0.10)
                        {
                            _logger.LogWarning($"ALERT: Parkplatz '{data.Name}' ist fast voll! Nur noch {data.FreeSpots} von {data.TotalSpots} frei ({ratio:P1}).");
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Fehler beim Verarbeiten der Kafka-Nachricht: {ex.Message}");
                }
            }
        }, stoppingToken);
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
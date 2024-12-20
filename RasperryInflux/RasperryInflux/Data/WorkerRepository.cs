using MQTTnet.Client;
using MQTTnet.Extensions.TopicTemplate;
using RasperryInflux.Data.InfluxDB;
using RasperryInflux.Entities;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace RasperryInflux.Data
{
    public class WorkerRepository : BackgroundService
    {
        static readonly MqttTopicTemplate sampleTemplate = new("zigbee2mqtt/0xa4c1380a1328ecf5");

        private readonly IInfluxRepository _influxDBService;

        private readonly IConfiguration _config;

        private readonly IMqttClient _mqttClient;

        private readonly MqttClientOptionsBuilder _mqttClientOptionsBuilder;


        public WorkerRepository(IInfluxRepository iInfluxDBService, IMqttClient mqttClient, MqttClientOptionsBuilder mqttClientOptions, ILogger<WorkerRepository> logger)
        {
            _influxDBService = iInfluxDBService;
            _mqttClient = mqttClient;
            _mqttClientOptionsBuilder = mqttClientOptions;

            _config = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken) { } //create more instance

        public async override Task StartAsync(CancellationToken stoppingToken) //creates one instance
        {
            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Telemetry? found = JsonSerializer.Deserialize<Telemetry>(Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment));
                _influxDBService.WriteTelemetry(found);
                return Task.CompletedTask;
            };
            await _mqttClient.ConnectAsync(_mqttClientOptionsBuilder.Build(), CancellationToken.None);

            var mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder().WithTopicTemplate(sampleTemplate).Build();

            await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        }
    }
}

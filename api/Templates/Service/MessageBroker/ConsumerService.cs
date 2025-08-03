using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using minifab.api.Templates.Service.SensorData;
using minifab.api.Templates.Models;

namespace minifab.api.Templates.Service.MessageBroker
{
    public interface IConsumerService
    {
        void StartListening();
    }

    public class ConsumerService : IConsumerService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private const string QueueName = "sensor_data_queue";

        public ConsumerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                RequestedConnectionTimeout = TimeSpan.FromSeconds(3)
            };

            try
            {
                Console.WriteLine("⏳ RabbitMQ bağlantısı kuruluyor...");
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: QueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                Console.WriteLine("✅ RabbitMQ bağlantısı başarılı!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ RabbitMQ bağlantı hatası: {ex.Message}");
                throw;
            }
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                // Her mesaj için yeni bir scope oluştur
                using var scope = _serviceProvider.CreateScope();
                var sensorDataService = scope.ServiceProvider.GetRequiredService<ISensorDataService>();

                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                try
                {
                    Console.WriteLine($"📥 Alınan mesaj: {json}");

                    var sensorData = JsonSerializer.Deserialize<SensorDataModel>(json);
                    if (sensorData != null)
                    {
                        try
                        {
                            await sensorDataService.AddSensorData(sensorData);
                            Console.WriteLine($"Veri başarıyla kaydedildi: Sıcaklık={sensorData.Temperature}°C, " +
                                            $"Nem=%{sensorData.Humidity}, Voltaj={sensorData.Voltage}V");
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine($" Geçersiz sensör verisi: {json}");
                        }
                    }
                }
                catch (JsonException jex)
                {
                    Console.WriteLine($"❌ JSON ayrıştırma hatası: {jex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Beklenmeyen hata: {ex.Message}");
                }
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: true,
                consumer: consumer
            );

            Console.WriteLine($"📡 '{QueueName}' kuyruğu dinleniyor...");
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}

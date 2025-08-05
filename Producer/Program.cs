using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MiniFab.Producer
{
    class Program
{
    private const string QueueName = "sensor_data_queue";
    private const int MessageIntervalMs = 5000; 

    static void Main()
    {
        while (true)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    Port = 5672,
                    UserName = "guest",
                    Password = "guest",
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(3)
                };

                Console.WriteLine(" RabbitMQ bağlantısı kuruluyor");

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: QueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                Console.WriteLine("RabbitMQ bağlantısı başarılı!");

                var random = new Random();

                while (connection.IsOpen)
                {
                    var data = new
                    {
                        DeviceId = "DEVICE-001",
                        Temperature = Math.Round(20 + random.NextDouble() * 10, 1),
                        Humidity = random.Next(30, 90),
                        Voltage = 220 + random.Next(0, 20),
                        Timestamp = DateTime.UtcNow
                    };

                    string json = JsonSerializer.Serialize(data);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: QueueName,
                        basicProperties: null,
                        body: body
                    );

                    Console.WriteLine($" Gönderilen veri: {json}");
                    Console.WriteLine($" {MessageIntervalMs/1000} saniye bekleniyor...");
                    Thread.Sleep(MessageIntervalMs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Hata: {ex.Message}");
                Console.WriteLine(" 5 saniye sonra tekrar denenecek...");
                Thread.Sleep(5000);
            }
        }
    }
}
}

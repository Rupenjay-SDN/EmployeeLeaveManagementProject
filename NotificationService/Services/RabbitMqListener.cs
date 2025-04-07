using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using NotificationService.Services;
using System.Text.Json;
using NotificationService.Models;
using System.Text;

public class RabbitMqListener : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;
    private readonly IServiceProvider _provider;

    public RabbitMqListener(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare both queues
        string[] queues = { "user-registration-queue", "leave-approval-queue" };

        foreach (var queue in queues)
        {
            _channel.QueueDeclare(queue, true, false, false, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var email = JsonSerializer.Deserialize<EmailMessage>(json);

                    using var scope = _provider.CreateScope();
                    var mailer = scope.ServiceProvider.GetRequiredService<EmailSenderService>();
                    await mailer.SendEmailAsync(email!);

                    Console.WriteLine($"✅ Email sent to {email?.To} from queue: {queue}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error processing message from {queue}: {ex.Message}");
                }
            };

            _channel.BasicConsume(queue, true, consumer);
        }

        return Task.CompletedTask;
    }
}

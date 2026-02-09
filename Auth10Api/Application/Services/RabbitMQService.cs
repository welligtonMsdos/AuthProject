using Auth10Api.Application.Interfaces;
using Auth10Api.Domain.Entities;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Auth10Api.Application.Services;

public class RabbitMQService : IRabbitMQService
{
    private readonly ConnectionFactory _factory;
    private const string QueueName = "trigger";

    public RabbitMQService(IConfiguration configuration)
    {
        _factory = new ConnectionFactory
        {
            HostName = configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost",           
            AutomaticRecoveryEnabled = true
        };
    }
    public async Task<bool> AddUserDtoAsync(User user)
    {
        await using var connection = await _factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var message = JsonSerializer.Serialize(user);
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: QueueName,
            mandatory: true,
            basicProperties: new BasicProperties { DeliveryMode = DeliveryModes.Persistent },
            body: body);

        return true;
    }
}

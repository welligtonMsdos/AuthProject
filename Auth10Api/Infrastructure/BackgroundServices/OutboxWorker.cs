using Auth10Api.Application.Interfaces;
using Auth10Api.Domain.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Auth10Api.Infrastructure.BackgroundServices;

public class OutboxWorker : BackgroundService
{
    private readonly IMongoClient _mongoClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxWorker> _logger;
    private readonly string _databaseName;

    public OutboxWorker(IMongoClient mongoClient,
                        ILogger<OutboxWorker> logger,
                        IConfiguration configuration,
                        IServiceProvider serviceProvider)
    {
        _mongoClient = mongoClient;        
        _logger = logger;
        _databaseName = configuration.GetValue<string>("MongoDB:DatabaseName")
                        ?? throw new ArgumentNullException("DatabaseName is not configured");
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var rabbitMQService = scope.ServiceProvider.GetRequiredService<IRabbitMQService>();

                await ProcessOutboxMessages(rabbitMQService, stoppingToken);
            }
          
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task ProcessOutboxMessages(IRabbitMQService rabbitMQService, 
                                             CancellationToken stoppingToken)
    {
        var collection = _mongoClient.GetDatabase(_databaseName)
                                     .GetCollection<OutboxMessage>("OutboxMessage");

        var pendingMessages = await collection
                .Find(m => m.ProcessedAt == null)
                .Limit(20) // Process in batches for performance
                .ToListAsync(stoppingToken);

        foreach (var message in pendingMessages)
        {
            try
            {
                _logger.LogInformation($"Processing Outbox {message.Id}");

                var userObject = JsonSerializer.Deserialize<User>(message.Content);

                if (userObject != null)
                {
                    bool sent = await rabbitMQService.AddUserDtoAsync(userObject);

                    if (sent)
                    {
                        var update = Builders<OutboxMessage>.Update
                                .Set(m => m.ProcessedAt, DateTime.UtcNow);

                        await collection.UpdateOneAsync(m => m.Id == message.Id, update);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing message {message.Id}");

                var errorUpdate = Builders<OutboxMessage>.Update
                    .Set(m => m.Error, ex.Message);

                await collection.UpdateOneAsync(m => m.Id == message.Id, errorUpdate);
            }
        }
    }
}
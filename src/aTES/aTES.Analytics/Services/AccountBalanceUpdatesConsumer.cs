using aTES.Analytics.DataAccess;
using aTES.SchemaRegistry;
using aTES.SchemaRegistry.Billing;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace aTES.Analytics.Services
{
    public class AccountBalanceUpdatesConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountBalanceUpdatesConsumer> _logger;
        private readonly MessageSerializer _serializer;


        public AccountBalanceUpdatesConsumer(IServiceProvider serviceProvider, ILogger<AccountBalanceUpdatesConsumer> logger, MessageSerializer serializer)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _serializer = serializer;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await DoExecute(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task DoExecute(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting events consumer");
            while (!Startup.DataBaseIdReady)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                await Task.Delay(1000, stoppingToken);
            }
            _logger.LogInformation("Consumer started");
            var conf = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "analytics-service",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _logger.LogInformation("Building config");
            using (var builder = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                _logger.LogInformation("Subscribing to {0}", Topics.ACCOUNT_BALANCE);
                builder.Subscribe(Topics.ACCOUNT_BALANCE);
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Waiting for new message");
                    var messageJson = builder.Consume(stoppingToken);
                    _logger.LogInformation("RECEIVED: {0}", messageJson.Message.Value);
                    var parsed = JObject.Parse(messageJson.Message.Value);
                    var version = parsed[nameof(IVersionedMessage<IMessagePayload>.EventVersion)].Value<string>();
                    if (version != "1")
                    {
                        _logger.LogError("Received unsupported version: {0}", version);
                    }
                    var eventType = parsed[nameof(IVersionedMessage<IMessagePayload>.EventType)].Value<string>();
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var serviceProvider = scope.ServiceProvider;
                        switch (eventType)
                        {
                            case AccountBalanceUpdatedMessage_v1.EVENT_TYPE:
                                var taskCreatedEvent = _serializer.Deserialize<AccountBalanceUpdatedMessage_v1>(messageJson.Message.Value);
                                await BalanceUpdated(taskCreatedEvent.Payload, serviceProvider);
                                break;
                            default:
                                //TODO: implement unknown version handling
                                _logger.LogError("Received unsupported type: {0}", eventType);
                                break;
                        }
                    }
                }
                _logger.LogInformation("Done");
            }
        }

        private async Task BalanceUpdated(AccountBalanceUpdatedMessage_v1.Data payload, IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<AnalyticsDbContext>();
            var user = await dbContext.Users.FirstAsync(x => x.PublicId == payload.UserId);
            user.Balance = payload.Balance;
            await dbContext.SaveChangesAsync();
        }
    }
}

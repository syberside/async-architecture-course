using aTES.Billing.DataAccess;
using aTES.SchemaRegistry;
using aTES.SchemaRegistry.Billing;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aTES.Billing.Services
{
    public class BillingCycleConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BillingCycleConsumer> _logger;
        private readonly MessageSerializer _serializer;

        public BillingCycleConsumer(IServiceProvider serviceProvider, ILogger<BillingCycleConsumer> logger, MessageSerializer serializer)
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
                GroupId = "tasks-service",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _logger.LogInformation("Building config");
            using (var builder = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                _logger.LogInformation("Subscribing to {0}", Topics.BILLING_CYCLE);
                builder.Subscribe(Topics.BILLING_CYCLE);
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Waiting for new message");
                    var messageJson = builder.Consume(stoppingToken);
                    _logger.LogInformation("RECEIVED: {0}", messageJson.Message.Value);
                    var parsed = JObject.Parse(messageJson.Message.Value);
                    var version = parsed[nameof(IVersionedMessage<IMessagePayload>.EventVersion)].Value<string>();
                    //var message = _serializer.Deserialize<MessageHolder>(messageJson.Message.Value);
                    if (version != "1")
                    {
                        //TODO: implement unknown version handling
                        _logger.LogError("Received unsupported version: {0}", version);
                    }
                    var eventType = parsed[nameof(IVersionedMessage<IMessagePayload>.EventType)].Value<string>();
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var serviceProvider = scope.ServiceProvider;
                        switch (eventType)
                        {
                            case OperationDayClosedMessage_v1.EVENT_TYPE:
                                var @event = _serializer.Deserialize<OperationDayClosedMessage_v1>(messageJson.Message.Value);
                                await ProcessCloseDay(@event.Payload, serviceProvider);
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

        private async Task ProcessCloseDay(OperationDayClosedMessage_v1.Data payload, IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<BillingDbContext>();
            var transactionLogService = serviceProvider.GetRequiredService<AccountingService>();
            var messageBus = serviceProvider.GetRequiredService<MessageBus>();
            var usersToPay = await dbContext.Users.Where(x => x.Balance > 0).ToArrayAsync();
            foreach (var user in usersToPay)
            {
                //NOTE: full implementation will be following:
                // - send event about payment to user X with amount Y
                // - subscriber 1 will perform 3rd party integration
                // - when async payment process will be done - new event "payment processed" will be triggered
                // - "payment processed" subscriber 1 will update user balance and transactions log
                // - "payment processed" subscriber 2 will send an email
                // Instead of full workflow we will just log payment fact and update transactions log
                _logger.LogInformation($"Payment: {user.Balance}");
                await transactionLogService.LogTransaction(user.PublicId, 0, user.Balance, $"End of day payment: {payload.Day}");
            }
        }
    }
}

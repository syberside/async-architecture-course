using aTES.Analytics.DataAccess;
using aTES.SchemaRegistry;
using aTES.SchemaRegistry.Tasks;
using aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace aTES.Analytics.Services
{
    public class TasksBillingConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TasksBillingConsumer> _logger;
        private readonly MessageSerializer _serializer;

        public TasksBillingConsumer(IServiceProvider serviceProvider, ILogger<TasksBillingConsumer> logger, MessageSerializer serializer)
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
                _logger.LogInformation("Subscribing to {0}", Topics.TASKS_BILLING);
                builder.Subscribe(Topics.TASKS_BILLING);
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
                            case TaskEstimatedMessage_v1.EVENT_TYPE:
                                var taskCreatedEvent = _serializer.Deserialize<TaskEstimatedMessage_v1>(messageJson.Message.Value);
                                await TaskEstimated(taskCreatedEvent.Payload, serviceProvider, taskCreatedEvent.EventCreatedAt);
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

        private async Task TaskEstimated(TaskEstimatedMessage_v1.Data data, IServiceProvider serviceProvider, DateTime eventCreatedAt)
        {
            var dbContext = serviceProvider.GetRequiredService<AnalyticsDbContext>();
            var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.PublicId == data.Id);
            if (task == null)
            {
                task = new DbTask
                {
                    Id = Guid.NewGuid(),
                    PublicId = data.Id,
                };
                dbContext.Tasks.Add(task);
            }
            task.Cost = data.MilletInABowlCost;
            //NOTE: dirty solution
            task.ClosedAt= eventCreatedAt;
            await dbContext.SaveChangesAsync();
        }
    }
}

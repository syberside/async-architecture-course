using aTES.SchemaRegistry;
using aTES.SchemaRegistry.Tasks;
using aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace aTES.Billing.Services
{
    public class TasksStreamConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TasksStreamConsumer> _logger;
        private readonly MessageSerializer _serializer;

        public TasksStreamConsumer(IServiceProvider serviceProvider, ILogger<TasksStreamConsumer> logger, MessageSerializer serializer)
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
                _logger.LogInformation("Subscribing to {0}", Topics.TASKS_WORKFLOW);
                builder.Subscribe(Topics.TASKS_WORKFLOW);
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
                            case TaskCreatedMessage_v1.EVENT_TYPE:
                                var taskCreatedEvent = _serializer.Deserialize<TaskCreatedMessage_v1>(messageJson.Message.Value);
                                await TaskCreated(taskCreatedEvent.Payload, serviceProvider);
                                break;

                            case BirdInACageMessage_v1.EVENT_TYPE:
                                var birdInACageEvent = _serializer.Deserialize<BirdInACageMessage_v1>(messageJson.Message.Value);
                                await BirdInACage(birdInACageEvent.Payload, serviceProvider);
                                break;
                            case MilletInABowlMessage_v1.EVENT_TYPE:
                                var milletInABowl = _serializer.Deserialize<MilletInABowlMessage_v1>(messageJson.Message.Value);
                                await MilletInABowl(milletInABowl.Payload, serviceProvider);
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

        private async Task MilletInABowl(MilletInABowlMessage_v1.Data data, IServiceProvider serviceProvider)
        {
            var accountingService = serviceProvider.GetRequiredService<AccountingService>();
            var taskService = serviceProvider.GetRequiredService<TasksService>();
            var task = await taskService.GetById(data.Id);
            await accountingService.LogTransaction(task.AssigneeId, task.MilletInABowlCost, 0, "Millet in a bowl: " + task.FullName);
        }

        private async Task BirdInACage(BirdInACageMessage_v1.Data data, IServiceProvider serviceProvider)
        {
            var accountingService = serviceProvider.GetRequiredService<AccountingService>();
            var taskService = serviceProvider.GetRequiredService<TasksService>();
            await taskService.UpdateAssignee(data.Id, data.AssigneeId);
            var task = await taskService.GetById(data.Id);
            await accountingService.LogTransaction(data.AssigneeId, task.BirdInCageCost, 0, "Bird in a cage: " + task.FullName);
        }

        private async Task TaskCreated(TaskCreatedMessage_v1.Data data, IServiceProvider serviceProvider)
        {
            var taskService = serviceProvider.GetRequiredService<TasksService>();
            await taskService.StoreAndEstimate(data.Id, data.JiraId, data.Description);
            //TODO: process CUD updates too
        }
    }
}

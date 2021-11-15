using aTES.SchemaRegistry.Tasks;
using aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1;
using aTES.TaskTracker.DataLayer;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace aTES.TaskTracker.Services
{
    public class TasksStreamConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TasksStreamConsumer> _logger;

        public TasksStreamConsumer(IServiceProvider serviceProvider, ILogger<TasksStreamConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
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
                _logger.LogInformation("Subscribing to {0}", Topics.TASKS_WORKFLOW_LEGACY);
                builder.Subscribe(Topics.TASKS_WORKFLOW_LEGACY);
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Waiting for new message");
                    var messageJson = builder.Consume(stoppingToken);
                    _logger.LogInformation("RECEIVED: {0}", messageJson.Message.Value);

                    var typeDefinition = new
                    {
                        Id = Guid.Empty,
                        Description = string.Empty,
                        AssigneeId = string.Empty,
                    };
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var serviceProvider = scope.ServiceProvider;
                        var message = JsonConvert.DeserializeAnonymousType(messageJson.Message.Value, typeDefinition);
                        if (message.Description != null)
                        {
                            await ResendTaskCreated(serviceProvider, message.Id, message.Description);
                        }
                        else if (message.AssigneeId != null)
                        {
                            await ResendBirdInACage(serviceProvider, message.Id, message.AssigneeId);
                        }
                        else
                        {
                            await ResendMilletInABowl(serviceProvider, message.Id);
                        }
                    }
                }
                _logger.LogInformation("Done");
            }
        }

        private async Task ResendMilletInABowl(IServiceProvider serviceProvider, Guid id)
        {
            await serviceProvider.GetRequiredService<MessageBus>().SendWorkflowEvent(new MilletInABowlMessage_v1
            {
                EventCreatedAt = DateTime.Now,
                EventId = Guid.NewGuid(),
                EventProducer = "TasksService",
                Payload = new MilletInABowlMessage_v1.Data
                {
                    Id = id.ToString(),
                }
            }, id.ToString());
        }

        private async Task ResendBirdInACage(IServiceProvider serviceProvider, Guid id, string assigneeId)
        {
            await serviceProvider.GetRequiredService<MessageBus>().SendWorkflowEvent(new BirdInACageMessage_v1
            {
                EventCreatedAt = DateTime.Now,
                EventId = Guid.NewGuid(),
                EventProducer = "TasksService",
                Payload = new BirdInACageMessage_v1.Data
                {
                    Id = id.ToString(),
                    AssigneeId = assigneeId,
                }
            }, id.ToString());
        }

        private async Task ResendTaskCreated(IServiceProvider serviceProvider, Guid id, string description)
        {
            var task = await serviceProvider.GetRequiredService<TasksDbContext>().Tasks.AsNoTracking().FirstAsync(x => x.Id == id);
            await serviceProvider.GetRequiredService<MessageBus>().SendWorkflowEvent(new TaskCreatedMessage_v1
            {
                EventCreatedAt = DateTime.Now,
                EventId = Guid.NewGuid(),
                EventProducer = "TasksService",
                Payload = new TaskCreatedMessage_v1.Data
                {
                    Id = id.ToString(),
                    Description = description,
                    JiraId = task.JiraId,
                }
            }, id.ToString());
        }
    }
}

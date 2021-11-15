using aTES.SchemaRegistry;
using aTES.SchemaRegistry.Tasks;
using aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1;
using aTES.TaskTracker.Domain;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TaskStatus = aTES.SchemaRegistry.Tasks.TaskStatus;

namespace aTES.TaskTracker.Services
{
    public class MessageBus : IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<MessageBus> _logger;
        private readonly MessageSerializer _serializer;
        //TODO: RENAME EVERYTHING
        public MessageBus(ILogger<MessageBus> logger, MessageSerializer serializer)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
            _logger = logger;
            _serializer = serializer;
        }

        public async Task SendTaskUpdatedStreamEvent(ITask task)
        {
            var message = new TaskUpdatedMessage_V1
            {
                EventCreatedAt = DateTime.Now,
                EventId = Guid.NewGuid(),
                EventProducer = "TaskService",
                Payload = new TaskUpdatedMessage_V1.Data
                {
                    AssigneeId = task.AssigneePublicId,
                    Description = task.Description,
                    Id = task.PublicId,
                    JiraId = task.JiraId,
                    Status = task.IsCompeleted ? TaskStatus.Completed : TaskStatus.Assigned,
                }
            };
            await Send(message, Topics.TASKS_STREAMING, task.PublicId);
        }

        public async Task SendTaskCreatedEvent(ITask task)
        {
            await SendWorkflowEvent(new TaskCreatedMessage_v1
            {
                EventCreatedAt = DateTime.Now,
                EventId = Guid.NewGuid(),
                EventProducer = "TasksService",
                Payload = new TaskCreatedMessage_v1.Data
                {
                    Id = task.PublicId,
                    Description = task.Description,
                    JiraId = task.JiraId,
                }
            }, task.PublicId);
        }

        public async Task SendBirdInACageEvent(ITask task)
        {
            await SendWorkflowEvent(new BirdInACageMessage_v1
            {
                EventCreatedAt = DateTime.Now,
                EventId = Guid.NewGuid(),
                EventProducer = "TasksService",
                Payload = new BirdInACageMessage_v1.Data
                {
                    Id = task.PublicId,
                    AssigneeId = task.AssigneePublicId,
                }
            }, task.PublicId);
        }

        public async Task SendMilletInABowlEvent(ITask task)
        {
            await SendWorkflowEvent(new MilletInABowlMessage_v1
            {
                EventCreatedAt = DateTime.Now,
                EventId = Guid.NewGuid(),
                EventProducer = "TasksService",
                Payload = new MilletInABowlMessage_v1.Data
                {
                    Id = task.PublicId,
                }
            }, task.PublicId);
        }

        public async Task SendWorkflowEvent<T>(T message, string taskId) where T : IMessage
        {
            await Send(message, Topics.TASKS_WORKFLOW, taskId);
        }

        private async Task Send<T>(T message, string topicName, string key) where T : IMessage
        {
            var messageJson = _serializer.Serialize(message);
            var deliveryResult = await _producer.ProduceAsync(topicName, new Message<string, string>
            {
                Key = key,
                Value = messageJson,
            });
            _logger.LogInformation("Message {0} delivered to log {1} with offset {2} to partition {3} and status {4}",
                messageJson,
                topicName,
                deliveryResult.Offset,
                deliveryResult.Partition,
                deliveryResult.Status);
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}

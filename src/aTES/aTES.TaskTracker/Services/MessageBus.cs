using aTES.SchemaRegistry;
using aTES.SchemaRegistry.Tasks;
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
        //private readonly IProducer<string, UserUpdatedMessage> _producer;
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<MessageBus> _logger;
        private readonly Serializer _serializer;

        public MessageBus(ILogger<MessageBus> logger, Serializer serializer)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };
            //https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/JsonSerialization/Program.cs
            //_producer = new ProducerBuilder<string, UserUpdatedMessage>(config)
            //    .SetValueSerializer(new JsonSerializer<Person>(schemaRegistry, jsonSerializerConfig))
            //    .Build();
            _producer = new ProducerBuilder<string, string>(config).Build();
            _logger = logger;
            _serializer = serializer;
        }

        public async Task SendTaskUpdatedStreamEvent(ITask task)
        {
            var message = new TaskUpdatedMessage
            {
                Id = task.PublicId,
                Description = task.Description,
                AssigneeId = task.AssigneePublicId,
                Status = task.IsCompeleted ? TaskStatus.Completed : TaskStatus.Assigned,
            };
            await Send(message, Topics.TASKS_STREAMING_LEGACY, task.PublicId);
        }

        public async Task SendTaskCreatedEvent(ITask task)
        {
            var message = new TaskCreatedMessage
            {
                Id = task.PublicId,
                Description = task.Description,
            };
            await Send(message, Topics.TASKS_WORKFLOW_LEGACY, task.PublicId);
        }

        public async Task SendTaskAssignedEvent(ITask task)
        {
            var message = new TaskAssignedMessage
            {
                Id = task.PublicId,
                AssigneeId = task.AssigneePublicId,
            };
            await Send(message, Topics.TASKS_WORKFLOW_LEGACY, task.PublicId);
        }

        public async Task SendTaskCompletedEvent(ITask task)
        {
            var message = new TaskCompletedMessage
            {
                Id = task.PublicId,
            };
            await Send(message, Topics.TASKS_WORKFLOW_LEGACY, task.PublicId);
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

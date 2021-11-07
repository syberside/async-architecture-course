using aTES.TaskTracker.Domain;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace aTES.TaskTracker.Services
{
    public class MessageBus : IDisposable
    {
        //private readonly IProducer<string, UserUpdatedMessage> _producer;
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<MessageBus> _logger;

        public MessageBus(ILogger<MessageBus> logger)
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
        }

        public async Task SendTaskUpdatedCUDEvent(ITask task)
        {
            var message = new TaskUpdatedMessage
            {
                Id = task.PublicId,
                Description = task.Description,
                AssigneeId = task.AssigneePublicId,
                Status = task.IsCompeleted ? TaskStatus.Completed : TaskStatus.Assigned,
            };
            await Send(message, "tasks-cud", task.PublicId);
        }

        public async Task SendTaskCreatedEvent(ITask task)
        {
            var message = new TaskCreatedMessage
            {
                Id = task.PublicId,
                Description = task.Description,
            };
            await Send(message, "tasks", task.PublicId);
        }

        public async Task SendTaskAssignedEvent(ITask task)
        {
            var message = new TaskAssignedMessage
            {
                Id = task.PublicId,
                AssigneeId = task.AssigneePublicId,
            };
            await Send(message, "tasks", task.PublicId);
        }

        public async Task SendTaskCompletedEvent(ITask task)
        {
            var message = new TaskCompletedMessage
            {
                Id = task.PublicId,
            };
            await Send(message, "tasks", task.PublicId);
        }

        private async Task Send<T>(T message, string logName, string key)
        {
            var messageJson = JsonConvert.SerializeObject(message);
            var deliveryResult = await _producer.ProduceAsync(logName, new Message<string, string>
            {
                Key = key,
                Value = messageJson,
            });
            _logger.LogInformation("Message {0} delivered to log {1} with offset {2} to partition {3} and status {4}",
                messageJson,
                logName,
                deliveryResult.Offset,
                deliveryResult.Partition,
                deliveryResult.Status);
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }

        public class TaskUpdatedMessage
        {
            public string Id { get; set; }

            public string Description { get; set; }

            public string AssigneeId { get; set; }

            public TaskStatus Status { get; set; }
        }

        public class TaskCreatedMessage
        {
            public string Id { get; set; }
            public string Description { get; set; }
        }

        public class TaskAssignedMessage
        {
            public string Id { get; set; }
            public string AssigneeId { get; set; }
        }

        public class TaskCompletedMessage
        {
            public string Id { get; set; }
        }

        public enum TaskStatus
        {
            Assigned = 1,
            Completed = 2,
        }
    }
}

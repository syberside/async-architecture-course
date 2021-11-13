using aTES.Identity.Domain;
using aTES.SchemaRegistry;
using aTES.SchemaRegistry.Users;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DtoRole = aTES.SchemaRegistry.Users.Roles;

namespace aTES.Identity.Services
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

        public async Task SendUserUpdatedStreamEvent(IUser user)
        {
            var message = new UserUpdatedMessage
            {
                Id = user.PublicId,
                //Note: dirty solution, but enough for study project
                Role = (DtoRole)user.Role,
                Username = user.Username,
            };
            //var deliveryResult = await _producer.ProduceAsync("accounts-cud", new Message<string, UserUpdatedMessage>
            //{
            //    Key = user.PublicId,
            //    Value = message,
            //});
            var messageJson = _serializer.Serialize(message);
            var deliveryResult = await _producer.ProduceAsync(Topics.USERS_STREAM_LEGACY, new Message<string, string>
            {
                Key = user.PublicId,
                Value = messageJson,
            });
            _logger.LogInformation("Message {0} delivered with offset {1} to partition {2} and status {3}",
                messageJson,
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

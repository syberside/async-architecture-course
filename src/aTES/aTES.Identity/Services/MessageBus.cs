using aTES.Identity.Domain;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace aTES.Identity.Services
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

        public async Task SendUserUpdatedCUDEvent(IUser user)
        {
            var message = new UserUpdatedMessage
            {
                Id = user.PublicId,
                Role = user.Role,
                Username = user.Username,
            };
            //var deliveryResult = await _producer.ProduceAsync("accounts-cud", new Message<string, UserUpdatedMessage>
            //{
            //    Key = user.PublicId,
            //    Value = message,
            //});
            var messageJson = JsonConvert.SerializeObject(message);
            var deliveryResult = await _producer.ProduceAsync("accounts-cud", new Message<string, string>
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

        public class UserUpdatedMessage
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public Roles Role { get; set; }
        }
    }
}

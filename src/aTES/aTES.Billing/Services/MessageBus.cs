﻿using aTES.SchemaRegistry;
using aTES.SchemaRegistry.Tasks;
using aTES.SchemaRegistry.Tasks.BusinessEvents.Workflow.V1;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace aTES.Billing.Services
{
    public class MessageBus
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<MessageBus> _logger;
        private readonly MessageSerializer _serializer;

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

        public async Task SendTaskEstimatedEvent(string publicId, int birdInCageCost, int milletInABowlCost)
        {
            var message = new TaskEstimatedMessage_v1
            {
                EventCreatedAt = DateTime.Now,
                EventId = Guid.NewGuid(),
                EventProducer = "BillingService",
                Payload = new TaskEstimatedMessage_v1.Data
                {
                    Id = publicId,
                    BirdInACageCost = birdInCageCost,
                    MilletInABowlCost = milletInABowlCost,
                }
            };
            var messageJson = _serializer.Serialize(message);
            var deliveryResult = await _producer.ProduceAsync(Topics.TASKS_BILLING, new Message<string, string>
            {
                Key = publicId,
                Value = messageJson,
            });
            _logger.LogInformation("Message {0} delivered with offset {1} to partition {2} and status {3}",
                messageJson,
                deliveryResult.Offset,
                deliveryResult.Partition,
                deliveryResult.Status);
        }
    }
}

using aTES.Billing.DataAccess;
using aTES.SchemaRegistry;
using aTES.SchemaRegistry.Users;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aTES.Billing.Services
{
    public class AccountsStreamConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountsStreamConsumer> _logger;
        private readonly MessageSerializer _serializer;


        public AccountsStreamConsumer(IServiceProvider serviceProvider, ILogger<AccountsStreamConsumer> logger, MessageSerializer serializer)
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
                GroupId = "billing-service",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _logger.LogInformation("Building config");
            using (var builder = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                _logger.LogInformation("Subscribing to {0}", Topics.USERS_STREAM_LEGACY);
                builder.Subscribe(Topics.USERS_STREAM_LEGACY);
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Waiting for new message");
                    var messageJson = builder.Consume(stoppingToken);
                    _logger.LogInformation("RECEIVED: {0}", messageJson.Message.Value);
                    var message = _serializer.Deserialize<UserUpdatedMessage>(messageJson.Message.Value);
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<BillingDbContext>();

                        var user = await dbContext.Users.Where(x => x.PublicId == message.Id).FirstOrDefaultAsync(stoppingToken);
                        if (user == null)
                        {
                            user = new DbUser
                            {
                                Id = Guid.NewGuid(),
                                PublicId = message.Id,
                            };
                            dbContext.Users.Add(user);
                        }
                        user.Username = message.Username;
                        //Note: dirty solution, but enough for study project
                        user.Role = (Domain.Roles)message.Role;
                        await dbContext.SaveChangesAsync();
                    }
                }
                _logger.LogInformation("Done");
            }
        }
    }
}

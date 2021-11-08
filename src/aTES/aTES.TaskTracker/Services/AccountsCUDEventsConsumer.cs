using aTES.TaskTracker.DataLayer;
using aTES.TaskTracker.Domain;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aTES.TaskTracker.Services
{
    public class AccountsCUDEventsConsumer : BackgroundService
    {
        private const string _topicName = "accounts-cud";
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountsCUDEventsConsumer> _logger;


        public AccountsCUDEventsConsumer(IServiceProvider serviceProvider, ILogger<AccountsCUDEventsConsumer> logger)
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
                _logger.LogInformation("Subscribing to {0}", _topicName);
                builder.Subscribe(_topicName);
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Waiting for new message");
                    var messageJson = builder.Consume(stoppingToken);
                    _logger.LogInformation("RECEIVED: {0}", messageJson.Message.Value);
                    var message = JsonConvert.DeserializeObject<UserUpdatedMessage>(messageJson.Message.Value);
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<TasksDbContext>();

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
                        user.Role = message.Role;
                        await dbContext.SaveChangesAsync();
                    }
                }
                _logger.LogInformation("Done");
            }
        }

        public class UserUpdatedMessage
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public Roles Role { get; set; }
        }
    }
}

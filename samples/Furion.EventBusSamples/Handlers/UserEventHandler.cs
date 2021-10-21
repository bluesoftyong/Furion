using Furion.EventBus;
using System.Text.Json;

namespace Furion.EventBusSamples.Handlers
{
    public class UserEventHandler : IEventHandler
    {
        private readonly ILogger<UserEventHandler> _logger;
        public UserEventHandler(ILogger<UserEventHandler> logger)
        {
            _logger = logger;
        }

        [EventSubscriber("User:Create")]
        public Task Create(EventSubscriberContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("新增用户：{User} {CallingTime}", JsonSerializer.Serialize(context.Source.Payload), context.CallingTime);
            return Task.CompletedTask;
        }

        [EventSubscriber("User:Update")]
        public Task Update(EventSubscriberContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("更新用户：{User} {CallingTime}", JsonSerializer.Serialize(context.Source.Payload), context.CallingTime);
            return Task.CompletedTask;
        }
    }
}
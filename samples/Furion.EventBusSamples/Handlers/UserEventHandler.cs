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
        public Task Create(EventSource eventSource, CancellationToken cancellationToken)
        {
            _logger.LogInformation("新增用户：{User}", JsonSerializer.Serialize(eventSource.Payload));
            return Task.CompletedTask;
        }

        [EventSubscriber("User:Update")]
        public Task Update(EventSource eventSource, CancellationToken cancellationToken)
        {
            _logger.LogInformation("更新用户：{User}", JsonSerializer.Serialize(eventSource.Payload));
            return Task.CompletedTask;
        }
    }
}
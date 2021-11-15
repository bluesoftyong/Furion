using Furion.EventBus;
using System.Text.Json;

namespace Furion.EventBusSamples.Subscribers
{
    public class UserEventSubscriber : IEventSubscriber
    {
        private readonly ILogger<UserEventSubscriber> _logger;
        public UserEventSubscriber(ILogger<UserEventSubscriber> logger)
        {
            _logger = logger;
        }

        [EventSubscribe("User:Create")]
        public Task Create(EventExecutingContext context)
        {
            _logger.LogInformation("新增用户：{User} {CreatedTime} - {CallingTime}", JsonSerializer.Serialize(context.Source.Payload), context.Source.CreatedTime, context.ExecutingTime);
            return Task.CompletedTask;
        }

        [EventSubscribe("User:Update")]
        public Task Update(EventExecutingContext context)
        {
            _logger.LogInformation("更新用户：{User} {CreatedTime} - {CallingTime}", JsonSerializer.Serialize(context.Source.Payload), context.Source.CreatedTime, context.ExecutingTime);
            return Task.CompletedTask;
        }

        [EventSubscribe("User:Delete")]
        public async Task Delete(EventExecutingContext context)
        {
            await Task.Delay(2000, context.Source.CancellationToken);
            throw new Exception("出问题了");
        }

        [EventSubscribe("User:Create")]
        [EventSubscribe("User:Update")]
        public Task CreateOrUpdate(EventExecutingContext context)
        {
            _logger.LogInformation("新增或更新触发");
            return Task.CompletedTask;
        }
    }
}
using Furion.EventBus;
using System.Text.Json;

namespace Furion.EventBusSamples.Subscriber
{
    public class UserEventSubscriber : IEventSubscriber, IEventSubscriberFilter
    {
        private readonly ILogger<UserEventSubscriber> _logger;
        public UserEventSubscriber(ILogger<UserEventSubscriber> logger)
        {
            _logger = logger;
        }

        [EventSubscribe("User:Create")]
        public Task Create(EventSubscribeExecutingContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("新增用户：{User} {CreatedTime} - {CallingTime}", JsonSerializer.Serialize(context.Source.Payload), context.Source.CreatedTime, context.ExecutingTime);
            return Task.CompletedTask;
        }

        [EventSubscribe("User:Update")]
        public Task Update(EventSubscribeExecutingContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("更新用户：{User} {CreatedTime} - {CallingTime}", JsonSerializer.Serialize(context.Source.Payload), context.Source.CreatedTime, context.ExecutingTime);
            return Task.CompletedTask;
        }

        [EventSubscribe("User:Delete")]
        public async Task Delete(EventSubscribeExecutingContext context, CancellationToken cancellationToken)
        {
            await Task.Delay(2000, cancellationToken);
            throw new Exception("出问题了");
        }

        [EventSubscribe("User:Create")]
        [EventSubscribe("User:Update")]
        public Task CreateOrUpdate(EventSubscribeExecutingContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("新增或更新触发");
            return Task.CompletedTask;
        }

        public Task OnHandlerExecutingAsync(EventSubscribeExecutingContext context)
        {
            _logger.LogInformation("执行之前：{EventId}", context.Source.EventId);
            return Task.CompletedTask;
        }

        public Task OnHandlerExecutedAsync(EventSubscribeExecutedContext context)
        {
            _logger.LogInformation("执行之后：{EventId}", context.Source.EventId);
            if (context.Exception != null)
            {
                _logger.LogError(context.Exception, "执行出错啦：{EventId}", context.Source.EventId);
            }
            return Task.CompletedTask;
        }
    }
}
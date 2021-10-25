using Furion.EventBus;

namespace Furion.EventBusSamples;

public class EventHandlerMonitor : IEventHandlerMonitor
{
    private readonly ILogger<EventHandlerMonitor> _logger;
    public EventHandlerMonitor(ILogger<EventHandlerMonitor> logger)
    {
        _logger = logger;
    }

    public Task OnExecutingAsync(EventHandlerExecutingContext context)
    {
        _logger.LogInformation("执行之前：{EventId}", context.Source.EventId);
        return Task.CompletedTask;
    }

    public Task OnExecutedAsync(EventHandlerExecutedContext context)
    {
        _logger.LogInformation("执行之后：{EventId}", context.Source.EventId);
        if (context.Exception != null)
        {
            _logger.LogError(context.Exception, "执行出错啦：{EventId}", context.Source.EventId);
        }
        return Task.CompletedTask;
    }
}
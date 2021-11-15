using Furion.EventBus;

namespace Furion.UnitTests;

internal class TestEventHandlerMonitor : IEventHandlerMonitor
{
    public Task OnExecutedAsync(EventExecutedContext context)
    {
        ThreadStaticValue.MonitorValue += 1;
        return Task.CompletedTask;
    }

    public Task OnExecutingAsync(EventExecutingContext context)
    {
        ThreadStaticValue.MonitorValue += 1;
        return Task.CompletedTask;
    }
}
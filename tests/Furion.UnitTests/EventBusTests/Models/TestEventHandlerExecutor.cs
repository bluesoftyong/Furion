using Furion.EventBus;

namespace Furion.UnitTests;

internal class TestEventHandlerExecutor : IEventHandlerExecutor
{
    public async Task ExecuteAsync(EventExecutingContext context, Func<EventExecutingContext, Task> handler)
    {
        ThreadStaticValue.ExecutorValue += 1;
        await handler(context);
        ThreadStaticValue.ExecutorValue += 1;
    }
}
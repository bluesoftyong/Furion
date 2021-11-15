using Furion.EventBus;

namespace Furion.EventBusSamples;

public class EventHandlerExecutor : IEventHandlerExecutor
{
    public async Task ExecuteAsync(EventExecutingContext context, Func<EventExecutingContext, Task> handler)
    {
        if (context.Source.EventId == "User:Delete")
        {
            Console.WriteLine("我是执行器~~~~");
            await handler(context);
        }
        else
        {
            await handler(context);
        }
    }
}
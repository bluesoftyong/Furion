using Furion.EventBus;

namespace Furion.UnitTests;

public class TestEventSubscriber : IEventSubscriber
{
    private Object obj = new();

    [EventSubscribe("Unit:Test")]
    public Task CreateTest(EventExecutingContext context)
    {
        return Task.CompletedTask;
    }

    [EventSubscribe("Unit:Test2")]
    [EventSubscribe("Unit:Test3")]
    public Task CreateTest2(EventExecutingContext context)
    {
        return Task.CompletedTask;
    }

    [EventSubscribe("Unit:Publisher")]
    public Task TestPublisher(EventExecutingContext context)
    {
        lock (obj)
        {
            var i = Convert.ToInt32(context.Source.Payload);
            ThreadStaticValue.PublishValue += i;
        }
        return Task.CompletedTask;
    }
}

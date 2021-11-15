using Furion.EventBus;

namespace Furion.UnitTests;

public class TestOtherEventSubscriber : IEventSubscriber
{
    [EventSubscribe("Unit:Other:Test")]
    public Task CreateTest(EventExecutingContext context)
    {
        return Task.CompletedTask;
    }
}
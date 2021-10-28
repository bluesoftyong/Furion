using Furion.EventBus;

namespace Furion.UnitTests;

internal class TestOtherEventSubscriber : IEventSubscriber
{
    [EventSubscribe("Unit:Other:Test")]
    public Task CreateTest(EventHandlerExecutingContext context)
    {
        return Task.CompletedTask;
    }
}
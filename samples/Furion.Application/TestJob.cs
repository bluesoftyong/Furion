using Furion.Schedule;

namespace Furion.Application;

public class TestJob : IJob
{
    public async Task ExecuteAsync(object context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}

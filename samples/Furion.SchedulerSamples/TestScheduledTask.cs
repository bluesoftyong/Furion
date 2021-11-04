using Furion.Scheduler;

namespace Furion.SchedulerSamples
{
    public class TestScheduledTask : IScheduledTask
    {
        public string Schedule => "* * * * *";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            await Task.CompletedTask;
        }
    }
}